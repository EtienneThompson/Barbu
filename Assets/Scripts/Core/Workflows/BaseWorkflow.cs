namespace Barbu.Core.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models;
    using Barbu.Models.Workflows;

    public abstract class BaseWorkflow<T> : IWorkflow, IDisposable
    {
        protected abstract Dictionary<string, IStep<T>> Steps { get; }

        protected bool IsPaused { get; set; }

        protected string currentStepName { get; set; }

        protected StepArguments<T> Arguments { get; set; }

        protected IEventsController eventsController;
        protected IStateMachine stateMachine;
        protected ITelemetryService telemetryService;

        private Dictionary<EventNames, Action<EventNames>> waitingHandlers;
        private Dictionary<EventNames, Action<EventNames, object>> waitingHandlersWithData;

        protected BaseWorkflow(IEventsController eventsController, IStateMachine stateMachine, ITelemetryService telemetryService)
        {
            this.eventsController = eventsController;
            this.stateMachine = stateMachine;
            this.telemetryService = telemetryService;

            this.waitingHandlers = new Dictionary<EventNames, Action<EventNames>>();
            this.waitingHandlersWithData = new Dictionary<EventNames, Action<EventNames, object>>();

            this.eventsController.Subscribe(EventNames.PauseGame, this.Pause);
            this.eventsController.Subscribe(EventNames.ResumeGame, this.ResumeEntireGame);
        }

        public async Task StartAsync()
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Start Executing workflow");
            if (this.currentStepName == null)
            {
                this.telemetryService.LogError("[BaseWorkflow] No step name was initialized!");
                throw new InvalidOperationException("A valid step must be set as the starting step name.");
            }

            this.IsPaused = false;
            while (this.currentStepName != null)
            {
                if (!this.Steps.ContainsKey(this.currentStepName))
                {
                    this.telemetryService.LogError($"[BaseWorkflow] Step {this.currentStepName} is not registered for this workflow!");
                    throw new InvalidOperationException($"Step {this.currentStepName} is not registered for this workflow!");
                }

                var step = this.Steps[this.currentStepName];
                this.telemetryService.LogInfo("[BaseWorkflow] Initializing step");
                step.Initialize(this, this.eventsController, this.stateMachine, this.telemetryService);
                this.telemetryService.LogInfo("[BaseWorkflow] Starting step");
                await step.InvokeAsync(this.Arguments);

                if (this.stateMachine.IsGamePaused() || this.IsPaused)
                {
                    this.telemetryService.LogInfo("[BaseWorkflow] Pausing workflow");
                    break;
                }
            }

            if (!this.IsPaused)
            {
                this.telemetryService.LogInfo("[BaseWorkflow] Workflow has completed");
                await this.OnWorkflowEnd();
            }
        }

        protected virtual Task OnWorkflowEnd()
        {
            this.Dispose();
            return Task.CompletedTask;
        }

        public void Pause(EventNames eventName = EventNames.PauseGame)
        {
            this.IsPaused = true;
        }

        public void WaitForEvent(EventNames eventName)
        {
            this.telemetryService.LogInfo($"[BaseWorkflow] Waiting for event {eventName} without data");
            this.Pause();
            Action<EventNames> waitingHandler = async (EventNames name) => await this.ReceiveEvent(name);
            this.waitingHandlers.Add(eventName, waitingHandler);
            this.eventsController.Subscribe(eventName, waitingHandler);
        }

        public void WaitForEventWithData(EventNames eventName)
        {
            this.telemetryService.LogInfo($"[BaseWorkflow] Waiting for event {eventName} with data");
            this.Pause();
            Action<EventNames, object> waitingHandlerWithData = async (EventNames name, object data) => await this.ReceiveEvent(name, data);
            this.waitingHandlersWithData.Add(eventName, waitingHandlerWithData);
            this.eventsController.Subscribe(eventName, waitingHandlerWithData);
        }

        public void SetNextStep(string stepName)
        {
            if (!string.IsNullOrEmpty(stepName) && !this.Steps.ContainsKey(stepName))
            {
                throw new ArgumentException($"The step name {stepName} does not exist in this workflow.");
            }

            this.currentStepName = stepName;
        }

        public void Dispose()
        {
            this.eventsController.Unsubscribe(EventNames.PauseGame, this.Pause);
            this.eventsController.Unsubscribe(EventNames.ResumeGame, this.ResumeEntireGame);

            foreach (var key in this.waitingHandlers.Keys)
            {
                if (this.waitingHandlers.TryGetValue(key, out var handler))
                {
                    this.eventsController.Unsubscribe(key, handler);
                }
            }

            foreach (var key in this.waitingHandlersWithData.Keys)
            {
                if (this.waitingHandlersWithData.TryGetValue(key, out var handler))
                {
                    this.eventsController.Unsubscribe(key, handler);
                }
            }
        }

        private async Task ReceiveEvent(EventNames eventName)
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Received event without data");
            if (this.waitingHandlers.TryGetValue(eventName, out var handler))
            {
                this.eventsController.Unsubscribe(eventName, handler);
                this.waitingHandlers.Remove(eventName);
            }

            // Only resume the workflow when the event is received if the entire game is not paused.
            this.IsPaused = false;
            if (!this.stateMachine.IsGamePaused())
            {
                await this.StartAsync();
            }
        }

        private async Task ReceiveEvent(EventNames eventName, object data)
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Received event");
            this.Arguments.EventData = data;
            if (this.waitingHandlersWithData.TryGetValue(eventName, out var handler))
            {
                this.eventsController.Unsubscribe(eventName, handler);
                this.waitingHandlersWithData.Remove(eventName);
            }
            await this.ReceiveEvent(eventName);
        }

        private void ResumeEntireGame(EventNames eventName = EventNames.ResumeGame)
        {
            // When the game is resumed, only start the workflow if not paused for some
            // other reason.
            this.telemetryService.LogInfo("[BaseWorkflow] Received resume event...");
            this.telemetryService.LogInfo($"[BaseWorkflow] IsPaused: {this.IsPaused}");
            if (!this.IsPaused)
            {
                Task _ = this.StartAsync();
            }
        }
    }
}
namespace Barbu.Core.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barbu.Interfaces.Core;
    using Barbu.Interfaces.Core.Workflows;
    using Barbu.Models;
    using Barbu.Models.Workflows;

    public abstract class BaseWorkflow<T> : IWorkflow
    {
        protected abstract Dictionary<string, IStep<T>> Steps { get; }

        protected bool IsPaused { get; set; }

        protected string currentStepName { get; set; }

        protected StepArguments<T> Arguments { get; set; }

        protected EventsController eventsController;
        protected ITelemetryService telemetryService;

        private EventNames waitingEventName;
        private Action waitingHandler;
        private Action<object> waitingHandlerWithData;

        protected BaseWorkflow()
        {
            this.eventsController = EventsController.GetInstance();
            this.telemetryService = TelemetryService.GetInstance();
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
                    throw new InvalidOperationException("The current step name is not registered for this workflow.");
                }

                var step = this.Steps[this.currentStepName];
                this.telemetryService.LogInfo("[BaseWorkflow] Initializing step");
                step.Initialize(this, this.telemetryService);
                this.telemetryService.LogInfo("[BaseWorkflow] Starting step");
                await step.InvokeAsync(this.Arguments);

                if (this.IsPaused)
                {
                    this.telemetryService.LogInfo("[BaseWorkflow] Pausing workflow");
                    break;
                }
            }

            this.telemetryService.LogInfo("[BaseWorkflow] Workflow has completed");
        }

        public void Pause()
        {
            this.IsPaused = true;
        }

        public void WaitForEvent(EventNames eventName)
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Waiting for event without data");
            this.Pause();
            this.waitingEventName = eventName;
            this.waitingHandler = async () => await this.ReceiveEvent();
            this.eventsController.Subscribe(eventName, this.waitingHandler);
        }

        public void WaitForEventWithData(EventNames eventName)
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Waiting for event with data");
            this.Pause();
            this.waitingEventName = eventName;
            this.waitingHandlerWithData = async (object data) => await this.ReceiveEvent(data);
            this.eventsController.Subscribe(eventName, this.waitingHandlerWithData);
        }

        public void SetNextStep(string stepName)
        {
            if (!string.IsNullOrEmpty(stepName) && !this.Steps.ContainsKey(stepName))
            {
                throw new ArgumentException($"The step name ${stepName} does not exist in this workflow.");
            }

            this.currentStepName = stepName;
        }

        private async Task ReceiveEvent()
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Received event without data");
            if (this.waitingHandlerWithData != null)
            {
                this.eventsController.Unsubscribe(this.waitingEventName, this.waitingHandler);
                this.waitingHandler = null;
            }

            this.IsPaused = false;
            await this.StartAsync();
        }

        private async Task ReceiveEvent(object data)
        {
            this.telemetryService.LogInfo("[BaseWorkflow] Received event");
            this.Arguments.EventData = data;
            this.eventsController.Unsubscribe(this.waitingEventName, this.waitingHandlerWithData);
            this.waitingHandlerWithData = null;
            await this.ReceiveEvent();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class BaseWorkflow<T> : IWorkflow
{
    protected abstract Dictionary<string, IStep> Steps { get; }

    protected bool IsPaused { get; set; }

    protected string currentStepName { get; set; }

    protected StepArguments<T> Arguments { get; set; }

    protected BaseWorkflow()
    {
    }

    public async Task StartAsync()
    {
        if (this.currentStepName == null)
        {
            throw new InvalidOperationException("A valid step must be set as the starting step name.");
        }

        this.IsPaused = false;
        while (this.currentStepName != null)
        {
            if (!this.Steps.ContainsKey(this.currentStepName))
            {
                throw new InvalidOperationException("The current step name is not registered for this workflow.");
            }

            var step = this.Steps[this.currentStepName];
            await step.InvokeAsync(this.Arguments);

            if (this.IsPaused)
            {
                break;
            }
        }
    }

    public void Pause()
    {
        this.IsPaused = true;
    }

    public void SetNextStep(string stepName)
    {
        if (!this.Steps.ContainsKey(stepName))
        {
            throw new ArgumentException($"The step name ${stepName} does not exist in this workflow.");
        }

        this.currentStepName = stepName;
    }
}
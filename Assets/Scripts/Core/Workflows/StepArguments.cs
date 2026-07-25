namespace Barbu.Core.Workflows
{
    public class StepArguments<T>
    {
        public StepArguments(IWorkflow workflow)
        {
            this.Workflow = workflow;
        }

        public IWorkflow Workflow { get; }

        public T Data { get; set; }

        public object EventData { get; set; }
    }
}
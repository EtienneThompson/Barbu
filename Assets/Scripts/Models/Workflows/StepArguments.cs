namespace Barbu.Models.Workflows
{
    public class StepArguments<T>
    {
        public T Data { get; set; }

        public object EventData { get; set; }
    }
}
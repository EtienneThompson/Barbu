namespace Barbu.Core.Workflows
{
    using System.Threading.Tasks;

    public interface IStep<T>
    {
        Task InvokeAsync(StepArguments<T> args);
    }
}
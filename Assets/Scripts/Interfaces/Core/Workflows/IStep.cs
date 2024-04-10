using System.Threading.Tasks;

public interface IStep
{
    void Initialize(IWorkflow workflow);

    Task InvokeAsync<T>(StepArguments<T> args);
}
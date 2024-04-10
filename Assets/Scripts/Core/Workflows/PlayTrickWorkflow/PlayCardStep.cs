using System.Threading.Tasks;

public class PlayCardStep : IStep
{
    private IWorkflow parentWorkflow;

    public void Initialize(IWorkflow workflow)
    {
        this.parentWorkflow = workflow;
    }

    public Task InvokeAsync<T>(StepArguments<T> args)
    {
        throw new System.NotImplementedException();
    }
}
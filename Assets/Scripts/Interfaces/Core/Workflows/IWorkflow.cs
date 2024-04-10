using System.Threading.Tasks;

public interface IWorkflow
{
    Task StartAsync();

    void Pause();

    void SetNextStep(string stepName);
}

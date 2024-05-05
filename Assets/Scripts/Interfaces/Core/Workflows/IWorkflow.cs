namespace Barbu.Interfaces.Core.Workflows
{
    using Barbu.Models;
    using System.Threading.Tasks;

    public interface IWorkflow
    {
        Task StartAsync();

        void Pause(EventNames eventName = EventNames.PauseGame);

        void WaitForEvent(EventNames eventName);

        void WaitForEventWithData(EventNames eventName);

        void SetNextStep(string stepName);
    }
}

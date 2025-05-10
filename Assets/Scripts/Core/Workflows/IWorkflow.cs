namespace Barbu.Core.Workflows
{
    using Barbu.Core.Events;
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

using Barbu;
using Barbu.Core;
using Barbu.Core.Events;
using Barbu.Core.Telemetry;
using Barbu.Core.Workflows;
using Barbu.Gameplay;
using Barbu.Gameplay.BoardState;
using Barbu.Gameplay.Rounds;
using Zenject;

public class InjectionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<IStateMachine>()
            .To<StateMachine>()
            .AsSingle();

        Container
            .Bind<ITelemetryService>()
            .To<TelemetryService>()
            .AsSingle();

        Container
            .Bind<IEventsController>()
            .To<EventsController>()
            .AsSingle();

        Container
            .Bind<ICardFactory>()
            .To<CardFactory>()
            .AsSingle();

        Container
            .Bind<IComputerStateFactory>()
            .To<ComputerStateFactory>()
            .AsSingle();

        Container
            .Bind<IWorkflowFactory>()
            .To<WorkflowFactory>()
            .AsSingle();
    }
}
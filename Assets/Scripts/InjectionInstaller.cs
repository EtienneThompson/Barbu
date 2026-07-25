using System.Collections.Generic;
using Barbu;
using Barbu.Core;
using Barbu.Core.Events;
using Barbu.Core.Telemetry;
using Barbu.Core.Workflows;
using Barbu.Core.Workflows.PlayTrickWorkflow;
using Barbu.Core.Workflows.RoundWorkflow;
using Barbu.Gameplay;
using Barbu.Gameplay.BoardState;
using Barbu.Gameplay.Rounds;
using Barbu.UI.Controllers;
using UnityEngine;
using Zenject;

public class InjectionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("Installing bindings...");
        Container
            .Bind<IStateMachine>()
            .To<StateMachine>()
            .AsSingle();

        Container
            .Bind<ITelemetryService>()
            .To<TelemetryService>()
            .AsSingle();

        Container
            .Bind<IStatisticsService>()
            .To<StatisticsService>()
            .AsSingle();

        Container
            .Bind<IEventsController>()
            .To<EventsController>()
            .AsSingle();

        Container
            .Bind<IDeck>()
            .To<Deck>()
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
            .Bind<IInGamePointsController>()
            .To<InGamePointsController>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<IGameBoard>()
            .To<GameBoard>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<IRoundOverlayController>()
            .To<RoundOverlayController>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<IScoreMenuController>()
            .To<ScoreMenuController>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<IAdvertisementController>()
            .To<AdvertisementController>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container.BindFactory<PlayCardStep, PlayCardStep.Factory>();
        Container.BindFactory<HandleCardPlayedStep, HandleCardPlayedStep.Factory>();
        Container.BindFactory<ResolveTrickStep, ResolveTrickStep.Factory>();
        Container.BindFactory<SetupRoundStep, SetupRoundStep.Factory>();
        Container.BindFactory<PreRoundStep, PreRoundStep.Factory>();
        Container.BindFactory<StartRoundStep, StartRoundStep.Factory>();
        Container.BindFactory<CleanupRoundStep, CleanupRoundStep.Factory>();
        Container.BindFactory<NextRoundStep, NextRoundStep.Factory>();
        Container.BindFactory<CompleteGameStep, CompleteGameStep.Factory>();
        Container.BindFactory<ShowAdvertisementStep, ShowAdvertisementStep.Factory>();
        Container.BindFactory<IRound, Dictionary<string, int[]>, Hand[], int, int, PlayTrickWorkflow, PlayTrickWorkflow.Factory>();
        Container.BindFactory<GameTypes, List<IRound>, RoundWorkflow, RoundWorkflow.Factory>();

        Container
            .Bind<IWorkflowFactory>()
            .To<WorkflowFactory>()
            .AsSingle();
        Debug.Log("Finished installing bindings");
    }
}
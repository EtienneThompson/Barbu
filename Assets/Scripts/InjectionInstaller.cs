using Barbu.Core;
using Barbu.Interfaces.Core;
using UnityEngine;
using Zenject;

public class InjectionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .Bind<ITelemetryService>()
            .To<TelemetryService>()
            .AsSingle();
    }
}
using _Project.Runtime.Core.Camera;
using Zenject;

namespace _Project.Runtime.Core.Installers
{
    public class CameraInstaller : MonoInstaller
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CameraPivotController>()
                .AsSingle()
                .NonLazy();
        }
    }
}
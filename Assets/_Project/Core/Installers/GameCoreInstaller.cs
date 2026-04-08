using _Project.Core.Main;
using UnityEngine;
using Zenject;

namespace _Project.Core.Installers
{
    public class GameCoreInstaller : MonoInstaller
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
            
            Debug.Log("Game services installed");
        }
    }
}
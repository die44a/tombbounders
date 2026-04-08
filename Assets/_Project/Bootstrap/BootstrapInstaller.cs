using _Project.Services;
using UnityEngine;
using Zenject;

namespace _Project.Bootstrap
{
    public class BootstrapInstaller : MonoInstaller
    {
        [Inject] private SceneLoaderService _sceneLoaderService;
        
        public override void InstallBindings()
        {
            _sceneLoaderService.LoadCoreScene();
        }
    }
}

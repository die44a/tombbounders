using System.Threading.Tasks;
using _Project.Global;
using _Project.Runtime.Player.Services;
using Zenject;

namespace _Project.Runtime.Core.Levels
{
    public class LevelFlowService : IInitializable
    {
        private readonly LevelController _levelController;
        private readonly PlayerSpawnService _spawnService;
        private readonly SceneFader _fader;

        public LevelFlowService(
            LevelController levelController, 
            PlayerSpawnService spawnService, 
            SceneFader fader)
        {
            _levelController = levelController;
            _spawnService = spawnService;
            _fader = fader;
        }

        void IInitializable.Initialize()
        {
            _levelController.LoadFirstLevel();
            
            var startPoint = _levelController.GetCurrentSpawnPoint();
            _spawnService.Spawn(startPoint);
        }

        public async void ChangeLevel() 
        {
            await _fader.FadeOutAsync(0.5f);

            _levelController.LoadNextLevel();

            var nextPoint = _levelController.GetCurrentSpawnPoint();
            _spawnService.Spawn(nextPoint);

            await Task.Delay(100); 

            await _fader.FadeInAsync(0.5f);
        }
    }
}
using System;
using _Project.Player.Runtime;
using _Project.Runtime.Core.Main.Interactables;
using _Project.Runtime.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.Global
{
    public class InteractionSetupService : IInitializable, IDisposable
    {
        private readonly DiContainer _container;

        public InteractionSetupService(DiContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            Debug.Log("[InteractionSetupService] Запуск мониторинга сцен.");
            SceneManager.sceneLoaded += OnSceneLoaded;
            PerformFullSetup();
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[InteractionSetupService] Сцена загружена: {scene.name}. Выполняю настройку.");
            PerformFullSetup();
        }

        private void PerformFullSetup()
        {
            SetupPlayer();
            SetupDoors();
            SetupUI();
        }

        private void SetupPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) player = Object.FindFirstObjectByType<PlayerMovementController>()?.gameObject;

            if (player != null)
            {
                if (player.GetComponent<PlayerInteractor>() == null)
                {
                    // Используем Zenject для создания компонента, чтобы пробросить IInputService
                    _container.InstantiateComponent<PlayerInteractor>(player);
                    Debug.Log($"[InteractionSetupService] PlayerInteractor добавлен на '{player.name}' через Container.");
                }
            }
        }

        private void SetupDoors()
        {
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            int doorCount = 0;
            
            foreach (var obj in allObjects)
            {
                if (obj.name.Contains("Door") || obj.name.Contains("Gate") || obj.GetComponent<Door>() != null)
                {
                    if (obj.GetComponent<Collider2D>() != null || obj.GetComponent<Animator>() != null)
                    {
                        if (obj.GetComponent<Door>() == null)
                        {
                            obj.AddComponent<Door>();
                            doorCount++;
                        }
                        
                        SetLayerRecursive(obj, 8); // Layer 8: Interactable
                    }
                }
            }
            
            if (doorCount > 0)
                Debug.Log($"[InteractionSetupService] Настроено дверей/ворот: {doorCount}");
        }

        private void SetupUI()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                if (canvas.GetComponent<InteractionHintUI>() == null)
                {
                    canvas.gameObject.AddComponent<InteractionHintUI>();
                }
            }
        }

        private void SetLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }
}

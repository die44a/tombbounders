using _Project.Runtime.Core.Main.Interfaces;
using _Project.Services;
using _Project.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using System.Linq;
using System.Collections;
using _Project.Runtime.Player.Controllers;

namespace _Project.Player.Runtime
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Настройки")] [SerializeField] private float _interactionRange = 100f;
        [SerializeField] private LayerMask _interactableLayer = 1 << 8; // Layer 8: Interactable

        [Header("Ссылки (Опционально)")] [SerializeField]
        private Camera _camera;

        private IInputService _inputService;
        private InputAction _interactAction;
        private IInteractable _currentInteractable;

        public IInteractable CurrentInteractable => _currentInteractable;


        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;

            _interactAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Interact);

            if (_interactAction != null)
            {
                _interactAction.started +=
                    _ => Debug.Log("1. [PlayerInteractor] Кнопка взаимодействия НАЖАТА"); // Этот основной
                // _interactAction.performed += OnInteractPerformed;
                _interactAction.canceled +=
                    _ => Debug.Log("2. [PlayerInteractor] Кнопка взаимодействия ОТПУЩЕНА"); // Этот основной
            }
            else
            {
                Debug.LogError($"3. [PlayerInteractor] Не найдено действие {PlayerActions.Interact}");
            }
        }

        private void OnDestroy()
        {
            if (_interactAction != null)
            {
                // _interactAction.performed -= OnInteractPerformed;
            }
        }

        private void Update()
        {
            CheckForInteractable();

            // Визуализация луча (только для отладки)
            if (_camera != null)
            {
                Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                Debug.DrawRay(ray.origin, ray.direction * _interactionRange, Color.green);
            }
        }

        private void CheckForInteractable()
        {
            IInteractable foundInteractable = null;

            if (_camera != null)
            {
                Ray ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, _interactionRange, _interactableLayer);
                Debug.Log(hit.collider?.gameObject.name);
                Debug.Log(hit.collider?.gameObject.layer);
                Debug.Log(hit.collider != null
                    ? $"Hit: {hit.collider.name}"
                    : "Hit: NULL");
                if (hit.collider != null)
                {
                    foundInteractable = hit.collider.GetComponent<IInteractable>();
                    if (foundInteractable != null)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
                    }
                }
            }


            // if (foundInteractable == null)
            // {   
            //     IInteractable closest = FindClosestInteractable();
            //     if (closest != null)
            //     {
            //         Debug.Log($"Вижу дверь видимо");
            //         foundInteractable = closest;
            //         _currentInteractable?.OnHoverExit(gameObject);
            //         _currentInteractable = foundInteractable;
            //         _currentInteractable?.OnHoverEnter(gameObject);
            //     }
            // }

            if (foundInteractable != _currentInteractable)
            {
                _currentInteractable?.OnHoverExit(gameObject);
                _currentInteractable = foundInteractable;
                _currentInteractable?.OnHoverEnter(gameObject);
            }
            // if (foundInteractable == null && _currentInteractable != null)
            // {
            //     _currentInteractable.OnHoverExit(gameObject);
            //     _currentInteractable = null;
            // }
        }

        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        // private void OnInteractPerformed(InputAction.CallbackContext context)
        // {
        //     Debug.Log("5. [PlayerInteractor] Сигнал Interact получен через Input System");
        //
        //     if (_playerController == null)
        //         return;
        //
        //     IInteractable target = _currentInteractable ?? FindClosestInteractable();
        //
        //     if (target == null)
        //     {
        //         Debug.LogWarning("8. [PlayerInteractor] Нажато E, но рядом нет ничего интерактивного.");
        //         return;
        //     }
        //
        //     _playerController.TryInteract(target);
        // }

        // private IEnumerator PerformInteraction()
        // {
        //     _playerController.BeginInteract();

        //     // 1. Взаимодействуем с тем, на что смотрим
        //     if (_currentInteractable != null)
        //     {
        //         Debug.Log($"6.  [PlayerInteractor] Взаимодействую с целью: {((MonoBehaviour)_currentInteractable).name}"); // Этот основной
        //         _currentInteractable.Interact(gameObject); // открывает дверь
        //         // return;
        //     }

        //     IInteractable closest = FindClosestInteractable();
        //     if (closest != null)
        //     {
        //         closest.Interact(gameObject); // закрывает дверь
        //     }
        //     else
        //     {
        //         Debug.LogWarning("8. [PlayerInteractor] Нажато E, но рядом нет ничего интерактивного."); // Этот основной
        //     }
        //     yield return new WaitForSeconds(0.0f);
        //     _playerController.EndInteract();
        // }

        [SerializeField] private float _maxSearchRadius = 10f;

        private readonly Collider2D[] _results = new Collider2D[16];

        public IInteractable FindClosestInteractable()
        {
            int count = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _maxSearchRadius,
                _results,
                _interactableLayer);

            IInteractable closest = null;
            float minDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                var col = _results[i];

                if (!col.TryGetComponent(out InteractableProxy proxy))
                    continue;

                var interactable = proxy.target as IInteractable;
                if (interactable == null)
                    continue;

                float dist = Vector2.Distance(transform.position, col.bounds.center);

                if (dist < minDistance && dist < interactable.GetInteractionDistance())
                {
                    minDistance = dist;
                    closest = interactable;
                }
            }

            return closest;
        }
    }
}
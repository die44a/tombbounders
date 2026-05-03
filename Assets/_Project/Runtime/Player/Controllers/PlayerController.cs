using System;
using System.Collections;
using _Project.Player.Runtime;
using _Project.Runtime.Core.Main;
using _Project.Runtime.Player.Main;
using _Project.Services;
using _Project.Services.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace _Project.Runtime.Player.Controllers
{
    public class PlayerController :
        MonoBehaviour,
        IPlayerStatus
    {
        public PlayerState currentState;
        public event Action<PlayerState> OnStateChanged;

        private PlayerMovementController _movementController;
        private Vector2 _moveInput;

        [Inject] private IInputService _inputService;

        public Vector2 MoveInput => _moveInput;
        public bool IsInvulnerableState => currentState == PlayerState.Dashing;

        private InputAction _moveAction;
        private InputAction _dashAction;
        private InputAction _interactAction;
        private PlayerInteractor _playerInteractor;

        private void Awake()
        {
            _movementController = GetComponent<PlayerMovementController>();
            _playerInteractor = GetComponentInChildren<PlayerInteractor>();
            Debug.Log(_playerInteractor == null ? "Interactor is NULL" : "Interactor OK");
        }

        private void Start()
        {
            _moveAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Move);
            _dashAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Dash);
            _interactAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Interact);

            _dashAction.performed += OnDashPerformed;
            _interactAction.performed += OnInteractPerformed;
        }

        private void OnDestroy()
        {
            _dashAction.performed -= OnDashPerformed;
            _interactAction.performed -= OnInteractPerformed;
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (currentState != PlayerState.Dashing && currentState != PlayerState.Interacting)
            {
                StartCoroutine(PerformDash());
            }
        }


        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("InteractPerformed");
            if (currentState is PlayerState.Idle or PlayerState.Walking)
            {
                SetState(PlayerState.Interacting);
                StartCoroutine(PerformIteraction());
            }
        }

        private IEnumerator PerformIteraction()
        {
            _playerInteractor.PerformInteraction();
            yield return new WaitForSeconds(0.0f);
            UpdateMoveState();
        }

        public void FixedUpdate()
        {
            if (currentState is PlayerState.Dashing or PlayerState.Interacting)
                return;

            _moveInput = _moveAction.ReadValue<Vector2>();
            UpdateMoveState();
            _movementController.ApplyMovement(_moveInput);
        }

        private void UpdateMoveState()
        {
            var targetState = _moveInput.sqrMagnitude > 0.01f
                ? PlayerState.Walking
                : PlayerState.Idle;

            SetState(targetState);
        }

        private IEnumerator PerformDash()
        {
            if (_moveInput.magnitude < 0.01f)
                yield break;

            SetState(PlayerState.Dashing);

            _movementController.Dash(_moveInput);

            yield return new WaitForSeconds(0.2f);

            UpdateMoveState();
        }

        private void SetState(PlayerState newState)
        {
            if (currentState == newState) return;
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }
    }
}
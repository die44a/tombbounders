using System;
using System.Collections;
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
        [Inject] private IInputService _inputService;
        [Inject] private IHealthObservable _healthObservable;
        
        public PlayerState CurrentState { get; private set; }
        
        public event Action<PlayerState> OnStateChanged;

        private PlayerMovementController _movementController;
        private Vector2 _moveInput;
        
        public Vector2 MoveInput => _moveInput;
        public bool IsInvulnerableState => CurrentState == PlayerState.Dashing;
        
        private InputAction _moveAction;
        private InputAction _dashAction;
        private InputAction _interactAction;

        private void Awake()
        {
            _movementController = GetComponent<PlayerMovementController>();
        }

        private void Start()
        {
            _moveAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Move);
            _dashAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Dash);
            _interactAction = _inputService.GetAction(InputMaps.Gameplay, PlayerActions.Interact);

            _dashAction.performed += OnDashPerformed;
            _interactAction.performed += OnInteractPerformed;
            
            _healthObservable.OnDeath += OnDeath;
        }

        private void OnDestroy()
        {
            _dashAction.performed -= OnDashPerformed;
            _interactAction.performed -= OnInteractPerformed;
            _healthObservable.OnDeath -= OnDeath;
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (CurrentState is PlayerState.Dashing 
                or PlayerState.Interacting
                or PlayerState.Dead
                || !_movementController.IsDashReady)
                return;
            
            StartCoroutine(PerformDash());
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (CurrentState is PlayerState.Dead)
                return;
            
            if (CurrentState is PlayerState.Idle or PlayerState.Walking)
                SetState(PlayerState.Interacting);
        }

        public void FixedUpdate()
        {
            if (CurrentState is PlayerState.Dashing 
                or PlayerState.Interacting 
                or PlayerState.Dead)
                return;

            _moveInput = _moveAction.ReadValue<Vector2>();
            UpdateMoveState();
            _movementController.ApplyMovement(_moveInput);
        }

        private void UpdateMoveState()
        {
            if (CurrentState == PlayerState.Dead)
                return; 
            
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
            if (CurrentState == newState) return;

            if (CurrentState == PlayerState.Dead && newState != PlayerState.Dead)
                return;
            
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }

        private void OnDeath()
        {
            SetState(PlayerState.Dead);
            _movementController.StopPhysics();
        }
        
        public void ResetPlayer(Vector3 spawnPosition)
        {
            StopAllCoroutines();

            CurrentState = PlayerState.Idle;
            _moveInput = Vector2.zero;

            _movementController.ResetMovement();
            _movementController.Stop();

            transform.position = spawnPosition;

            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
using System;
using _Project.Runtime.Interfaces;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Player.Controllers
{
    public class PlayerInteractorController : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 1.5f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private Vector2 originOffset = new (0, 0.5f);

        private PlayerController _playerController;
        
        private IInteractable _currentHoveredInteractable;
        private Collider2D _lastHitCollider;

        [Inject]
        public void Construct(PlayerController playerController)
        {
            _playerController = playerController;
        }

        private void FixedUpdate()
        {
            CheckForHover();
        }

        private void CheckForHover()
        {
            var origin = (Vector2)transform.position + originOffset;
            var direction = _playerController.LastDirection;
            var hit = Physics2D.Raycast(origin, direction, interactDistance, interactableLayer);

            Debug.DrawRay(origin, direction * interactDistance, hit.collider ? Color.red : Color.green);
            
            if (hit.collider == _lastHitCollider)
                return;

            _lastHitCollider = hit.collider;

            IInteractable foundInteractable = null;

            if (hit.collider)
            {
                hit.collider.TryGetComponent(out foundInteractable);
            }
    
            if (foundInteractable != _currentHoveredInteractable)
            {
                _currentHoveredInteractable?.OnHoverExit();
                _currentHoveredInteractable = foundInteractable;
                _currentHoveredInteractable?.OnHoverEnter();
            }
        }

        public void PerformInteraction()
            => _currentHoveredInteractable?.Interact(gameObject);
    }
}
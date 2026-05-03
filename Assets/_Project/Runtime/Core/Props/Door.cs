using System;
using _Project.Runtime.Interfaces;
using UnityEngine;

namespace _Project.Runtime.Core.Props
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Collider2D interactableCollider;
        
        public SpriteRenderer Renderer { get; private set; }
        private bool _isOpen;
        
        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            interactableCollider.isTrigger = false;
        }

        public void Interact(GameObject initiator, Action onComplete)
        {
            _isOpen = !_isOpen;
            interactableCollider.isTrigger = _isOpen;
            onComplete?.Invoke();
        }

        public bool IsInteractable => true;
        public string GetInteractionLabel() => "Open Door";
    }
}
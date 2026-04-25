using _Project.Runtime.Core.Main.Interfaces;
using UnityEngine;

namespace _Project.Runtime.Core.Main.Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour, IInteractable
    {
        private const float distanceInteract = 3f;

        // [Header("Настройки анимации")]
        [SerializeField] private Animator _animator; // начинаю возвращать

        [SerializeField] private Collider2D _interactionCollider; // новое
        public Collider2D InteractionCollider => _interactionCollider; // новое

        [Header("Настройки физики")]
        [SerializeField] private Collider2D _doorCollider;

        [Header("Текст")]
        [SerializeField] private string _openPrompt = "Открыть [E]";
        [SerializeField] private string _closePrompt = "Закрыть [E]";

        private bool _isOpen = false;
        // private Coroutine _animationRoutine;

        private void Awake()
        {
            SetLayerRecursive(gameObject, LayerMask.NameToLayer("Interactable")); // 8: Interactable
        }

        private void SetLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }

        private void Start()
        {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();  // начинаю возвращать   
            if (_doorCollider == null) _doorCollider = GetComponentInChildren<Collider2D>();

            // Инициализация: дверь закрыта
            _isOpen = false;
            if (_doorCollider != null) _doorCollider.enabled = true;
            
            // if (_animator != null)
            // {
            //     _animator.speed = 0;
            //     // Твои ассеты: 0 - открыто, 1 - закрыто. 
            //     // Ставим 1, чтобы дверь была закрыта при старте.
            //     _animator.Play(0, -1, 1f); 
            // }
            if (_animator != null) // начинаю возвращать   
                _animator.Play("DoorClose", 0, 0f); // начинаю возвращать   
            
            Debug.Log($"[Door] {name}: Инициализация (Инверсия). Старт в кадре 1.0 (ЗАКРЫТО).");
        }

        public string GetInteractionPrompt() => _isOpen ? _closePrompt : _openPrompt;
        public bool CanInteract(GameObject interactor) => true;

        public bool Interact(GameObject interactor) // НОРМ
        {
            _isOpen = !_isOpen;
            Debug.Log($"<color=cyan>[Door] {name}: Взаимодействие! IsOpen = {_isOpen}</color>");
            UpdateState();
            return true;
        }

        private void UpdateState()
        {
            // Физика
            if (_doorCollider != null)
            {
                _doorCollider.enabled = !_isOpen;
            }

            // if (_animator != null) // начинаю возвращать   
            // {
            //     if (_isOpen)
            //         _animator.Play("Door-Front-Open-Horizontal");
            //     else
            //         _animator.Play("Door-Front-Close-Horizontal");
            // } // начинаю возвращать   

            // Анимация
            // if (_animator != null)
            // {
            //     if (_animationRoutine != null) StopCoroutine(_animationRoutine);
            //     _animationRoutine = StartCoroutine(PlayDoorAnimation());
            // }
        }

        // private System.Collections.IEnumerator PlayDoorAnimation()
        // {
        //     // ИНВЕРСИЯ (0 - открыто, 1 - закрыто):
        //     // Если ОТКРЫВАЕМ (_isOpen = true): идем от 1 к 0. Speed = -1, Start = 1.
        //     // Если ЗАКРЫВАЕМ (_isOpen = false): идем от 0 к 1. Speed = 1, Start = 0.
            
        //     float targetSpeed = _isOpen ? -1f : 1f;
        //     float startNormalizedTime = _isOpen ? 1f : 0f;
            
        //     _animator.speed = targetSpeed;
        //     _animator.Play(0, -1, startNormalizedTime);

        //     // Ждем один кадр для обновления стейта
        //     yield return null;
            
        //     float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
        //     yield return new WaitForSeconds(duration);

        //     // Останавливаем в конечной точке
        //     _animator.speed = 0;
        //     _animationRoutine = null;
            
        //     Debug.Log($"[Door] {name}: Анимация (Инверсия) завершена. Текущий IsOpen: {_isOpen}");
        // }

        public void OnHoverEnter(GameObject interactor) { }
        public void OnHoverExit(GameObject interactor) { }
        public void SetHighlight(bool isActive){ }
        public float GetInteractionDistance(){ return distanceInteract; }
        public int GetInteractionPriority(){ return 0; }
    }
}

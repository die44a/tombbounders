using _Project.Player.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Runtime.UI
{
    /// <summary>
    /// Отображает текстовую подсказку (например, "[E] Открыть"), когда игрок может взаимодействовать с объектом.
    /// </summary>
    public class InteractionHintUI : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private GameObject _hintContainer;
        [SerializeField] private Text _hintText;

        private PlayerInteractor _playerInteractor;

        private void Start()
        {
            _playerInteractor = FindObjectOfType<PlayerInteractor>();
            
            if (_playerInteractor == null)
            {
                Debug.LogWarning("[InteractionHintUI] PlayerInteractor не найден на сцене.");
            }

            // Если ссылки не назначены, пробуем найти их или создать
            if (_hintContainer == null)
            {
                // Ищем в дочерних объектах
                var child = transform.Find("InteractionHint");
                if (child != null)
                {
                    _hintContainer = child.gameObject;
                    if (_hintText == null) _hintText = child.GetComponent<Text>();
                }
                else
                {
                    // Создаем базовый UI программно
                    CreateDefaultHint();
                }
            }

            if (_hintContainer != null)
            {
                _hintContainer.SetActive(false);
            }
        }

        private void CreateDefaultHint()
        {
            GameObject container = new GameObject("InteractionHint");
            container.transform.SetParent(this.transform, false);
            
            RectTransform rect = container.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, -150); // Чуть ниже центра
            rect.sizeDelta = new Vector2(400, 50);

            _hintText = container.AddComponent<Text>();
            _hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _hintText.fontSize = 32;
            _hintText.alignment = TextAnchor.MiddleCenter;
            _hintText.color = Color.white;

            // Добавляем тень для читаемости
            var shadow = container.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(2, -2);

            _hintContainer = container;
        }

        private void Update()
        {
            if (_playerInteractor == null) return;

            var interactable = _playerInteractor.CurrentInteractable;

            if (interactable != null)
            {
                if (_hintContainer != null && !_hintContainer.activeSelf)
                {
                    _hintContainer.SetActive(true);
                }

                if (_hintText != null)
                {
                    _hintText.text = interactable.GetInteractionPrompt();
                }
            }
            else
            {
                if (_hintContainer != null && _hintContainer.activeSelf)
                {
                    _hintContainer.SetActive(false);
                }
            }
        }
    }
}

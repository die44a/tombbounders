using UnityEngine;

namespace _Project.Runtime.Core.Main.Interfaces
{
    public interface IInteractable
    {
        // Текст подсказки для UI (например, "Открыть [E]")
        string GetInteractionPrompt();

        // Проверка, можно ли сейчас взаимодействовать
        bool CanInteract(GameObject interactor);

        // Само действие
        bool Interact(GameObject interactor);

        // Вызывается, когда игрок наводит прицел
        void OnHoverEnter(GameObject interactor);

        // Вызывается, когда игрок отводит прицел
        void OnHoverExit(GameObject interactor);

        // Включение/выключение визуальной подсветки объекта
        void SetHighlight(bool isActive);

        // Дистанция, на которой можно взаимодействовать с объектом
        float GetInteractionDistance();

        // Приоритет выбора при нескольких объектах
        int GetInteractionPriority();

        Collider2D InteractionCollider { get; }
    }
}

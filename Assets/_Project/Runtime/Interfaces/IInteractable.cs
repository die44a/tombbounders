using UnityEngine;

namespace _Project.Runtime.Interfaces
{
    public interface IInteractable
    {
        void Interact(GameObject initiator);
        float InteractionDistance { get; }
        bool IsInteractable { get; }
        string GetInteractionLabel();
        void OnHoverEnter();
        void OnHoverExit();
    }
}
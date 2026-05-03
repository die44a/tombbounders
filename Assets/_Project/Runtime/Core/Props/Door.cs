using _Project.Runtime.Interfaces;
using UnityEngine;

namespace _Project.Runtime.Core.Props
{
    public class Door : MonoBehaviour, IInteractable

    {
        public void Interact(GameObject initiator)
        {
            Debug.Log("Door interact");
        }

        public float InteractionDistance { get; }
        public bool IsInteractable { get; }
        
        public string GetInteractionLabel()
        {
            throw new System.NotImplementedException();
        }

        public void OnHoverEnter()
        {
            Debug.Log("Door hover enter");
        }

        public void OnHoverExit()
        {
            Debug.Log("Door hover exit");
        }
    }
}
using System;
using _Project.Runtime.Player.Controllers;
using UnityEngine;

namespace _Project.Runtime.Core.Maps
{
    [RequireComponent(typeof(Collider2D))]
    public class ExitPoint : MonoBehaviour
    {
        public event Action OnReached;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<PlayerController>(out _))
                OnReached?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
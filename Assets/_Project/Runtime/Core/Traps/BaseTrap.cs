using System.Collections.Generic;
using UnityEngine;

namespace _Project.Runtime.Core.Traps
{
    public abstract class BaseTrap : MonoBehaviour
    {
        [SerializeField] protected float damageAmount = 10f;

        protected readonly List<IDamageable> TargetsInRange = new ();

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
                if (!TargetsInRange.Contains(damageable))
                    TargetsInRange.Add(damageable);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
                TargetsInRange.Remove(damageable);
        }

        protected void DealDamageToAll()
        { 
            foreach (var target in TargetsInRange)
                target.ApplyDamage(damageAmount);
        }
    }
}
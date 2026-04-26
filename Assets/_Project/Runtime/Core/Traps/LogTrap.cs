using UnityEngine;

namespace _Project.Runtime.Core.Traps
{
    public class LogTrap : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 5f;
        [SerializeField] private float knockbackForce = 100f;

        [SerializeField] private bool applyKnockback = true;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var damageable = collision.gameObject.GetComponentInParent<IDamageable>();

            if (damageable == null) return;
            damageable.ApplyDamage(damageAmount);

            if (applyKnockback)
            {
                ApplyKnockbackEffect(collision);
            }
        }

        private void ApplyKnockbackEffect(Collision2D collision)
        {
            var rb = collision.gameObject.GetComponentInParent<Rigidbody2D>();

            if (rb == null) return;
            Vector2 direction = (collision.transform.position - transform.position).normalized;
                
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
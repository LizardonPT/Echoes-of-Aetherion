using EchoesOfEtherion.HealthSystem;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Core
{
    public class EnemyProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] protected LayerMask targetMask;
        [SerializeField] protected LayerMask environmentMask;
        [SerializeField] protected float radius = 1f;
        [SerializeField] protected float collisionCheckThreshold = 0.5f;
        [SerializeField] protected EventReference hitSound;

        [SerializeField] protected Transform visual;
        [SerializeField] protected float maxHeight = 3f;
        [SerializeField] protected float duration = 1f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float knockback = 150f;
        protected Vector3 startPos;
        protected Vector3 targetPos;
        protected float timer = 0f;
        protected bool hasReachedThreshold = false;

        public virtual void Initialize(Vector3 startPos, Vector3 targetPos)
        {
            this.startPos = startPos;
            this.targetPos = targetPos;
            timer = 0f;
            hasReachedThreshold = false;
            if (visual != null)
                visual.localPosition = Vector3.zero;
        }

        protected virtual void Update()
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration);

            // Move projectile along linear path
            transform.position = new Vector3(
                Mathf.Lerp(startPos.x, targetPos.x, progress),
                Mathf.Lerp(startPos.y, targetPos.y, progress),
                0f
            );

            // Height / arc movement
            if (visual != null)
            {
                float height = Mathf.Sin(progress * Mathf.PI) * maxHeight;
                Vector3 pos = visual.localPosition;
                pos.y = height;
                visual.localPosition = pos;

                if (!hasReachedThreshold && height >= collisionCheckThreshold)
                {
                    hasReachedThreshold = true;
                }
            }

            // Check collisions after reaching threshold
            if (hasReachedThreshold)
            {
                CheckCollisions();
            }

            if (progress >= 1f)
            {
                OnReachedTarget();
            }
        }

        protected virtual void CheckCollisions()
        {
            Vector3 pos = transform.position;

            // Targets
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius, targetMask);
            foreach (var hit in hits)
            {
                OnHitTarget(hit);
            }

            // Environment
            Collider2D[] envHits = Physics2D.OverlapCircleAll(pos, radius, environmentMask);
            if (envHits.Length > 0)
            {
                OnHitEnvironment();
            }
        }

        protected virtual void OnHitTarget(Collider2D hit)
        {
            if (!hitSound.IsNull)
                RuntimeManager.PlayOneShot(hitSound, transform.position);

            if (hit.TryGetComponent(out HealthModule health))
            {
                health.Damage(gameObject, damage, knockback);
            }

            Destroy(gameObject);
        }

        protected virtual void OnHitEnvironment()
        {
            if (!hitSound.IsNull)
                RuntimeManager.PlayOneShot(hitSound, transform.position);
            Destroy(gameObject);
        }

        protected virtual void OnReachedTarget()
        {
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (visual != null)
            {
                Gizmos.color = hasReachedThreshold ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(visual.position, radius);
            }
        }
#endif
    }
}

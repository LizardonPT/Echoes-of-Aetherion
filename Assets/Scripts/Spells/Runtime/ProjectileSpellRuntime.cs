using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.Spells.Data;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Spells.Runtime
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ProjectileSpellRuntime : MonoBehaviour
    {
        [SerializeField] private EventReference hitEnemyEventReference;
        [SerializeField] private EventReference hitEnvironmentEventReference;

        [SerializeField] private float radius = 16;

        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask environmentMask;

        public ProjectileSpell SpellInfo { get; private set; }

        private Rigidbody2D rb;
        private Vector2 originalPos;

        public bool IsActive { get; private set; } = false;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.Sleep();
        }

        public virtual void ExecuteSpell(ProjectileSpell spellInfo, Vector2 position, Vector2 direction)
        {
            this.SpellInfo = spellInfo;
            transform.position = position;
            originalPos = position;

            rb.WakeUp();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * spellInfo.Speed, ForceMode2D.Impulse);
            IsActive = true;
        }

        protected virtual void Update()
        {
            if (IsActive && Vector2.Distance(originalPos, transform.position) >= SpellInfo.Range)
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (IsActive)
            {
                RaycastHit2D[] enemyCollisions = Physics2D.CircleCastAll(
                    transform.position,
                    radius,
                    rb.linearVelocity.normalized,
                    rb.linearVelocity.magnitude * Time.fixedDeltaTime,
                    enemyMask
                );

                if (enemyCollisions.Length > 0)
                {
                    bool hit = false;
                    foreach (RaycastHit2D hit2D in enemyCollisions)
                    {
                        if (hit2D.collider.TryGetComponent(out HealthModule health))
                        {
                            health.Damage(gameObject, SpellInfo.Damage, SpellInfo.KnockbackAmount, 0.25f);
                            hit = true;
                        }
                    }
                    if (hit)
                        OnHitEnemy();
                }

                RaycastHit2D environmentCollision = Physics2D.CircleCast(
                    transform.position,
                    radius,
                    rb.linearVelocity.normalized,
                    rb.linearVelocity.magnitude * Time.fixedDeltaTime,
                    environmentMask
                );

                if (environmentCollision.collider != null)
                    OnHitEnvironment();

            }
        }

        protected virtual void OnHitEnvironment()
        {
            if (!string.IsNullOrEmpty(hitEnvironmentEventReference.Path))
                RuntimeManager.PlayOneShot(hitEnvironmentEventReference, transform.position);

            IsActive = false;
            Destroy(gameObject);
        }

        protected virtual void OnHitEnemy()
        {
            if (!string.IsNullOrEmpty(hitEnemyEventReference.Path))
                RuntimeManager.PlayOneShot(hitEnemyEventReference, transform.position);
                
            IsActive = false;
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif

    }
}

using EchoesOfEtherion.HealthSystem;
using EchoesOfEtherion.Spells.Data;
using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Spells.Runtime
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LightBallSpellRuntime : MonoBehaviour
    {
        [SerializeField] private EventReference hitEventReference;
        [SerializeField] private LightBallSpell spellData;

        [SerializeField] private float radius = 16;

        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask environmentMask;
        private Rigidbody2D rb;
        private Vector2 originalPos;

        public bool IsActive { get; private set; } = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.Sleep();
        }

        public void ExecuteSpell(Vector2 position, Vector2 direction)
        {
            transform.position = position;
            originalPos = position;

            rb.WakeUp();
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * spellData.Speed, ForceMode2D.Impulse);
            IsActive = true;
        }

        private void Update()
        {
            if (IsActive && Vector2.Distance(originalPos, transform.position) >= spellData.Range)
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
                            health.Damage(gameObject, spellData.Damage, spellData.KnockbackAmount, 0.25f);
                            hit = true;
                        }
                    }
                    if (hit)
                        RuntimeManager.PlayOneShot(hitEventReference, transform.position);
                    IsActive = false;

                    Destroy(gameObject);
                }

                RaycastHit2D environmentCollision = Physics2D.CircleCast(
                    transform.position,
                    radius,
                    rb.linearVelocity.normalized,
                    rb.linearVelocity.magnitude * Time.fixedDeltaTime,
                    environmentMask
                );

                if (environmentCollision.collider != null)
                    Destroy(gameObject);
            }
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

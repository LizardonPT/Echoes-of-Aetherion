using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LightBall : MonoBehaviour, IProjectileSpell
    {
        [SerializeField] private SpellPage spellData;

        [SerializeField] private float speed = 10f;

        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask environmentMask;
        private Rigidbody2D rb;

        public bool IsActive { get; private set; } = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void ExecuteSpell(GameObject caster, Vector2 direction)
        {
            if (rb != null)
            {
                transform.position = caster.transform.position;
                rb.WakeUp();
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(direction * speed, ForceMode2D.Impulse);
            }
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            // Check for enemy collision
        }


    }
}

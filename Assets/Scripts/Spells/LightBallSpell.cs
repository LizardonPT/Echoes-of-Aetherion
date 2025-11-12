using UnityEngine;

namespace EchoesOfEtherion.Spells
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class LightBallSpell : MonoBehaviour, IProjectileSpell
    {
        [SerializeField] private SpellPage spellData;

        [SerializeField] private float speed = 10f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private int range = 50;

        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask environmentMask;
        private Rigidbody2D rb;
        private Vector2 originalPos;

        public bool IsActive { get; private set; } = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void ExecuteSpell(Vector2 position, Vector2 direction)
        {
            if (rb != null)
            {
                transform.position = position;
                originalPos = position;
                
                rb.WakeUp();
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(direction * speed, ForceMode2D.Impulse);
                IsActive = true;
            }
            else
            {
                Debug.LogError("[LightBallSpell] Doesn't contain a RigidBody2D!");
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (IsActive && Vector2.Distance(originalPos, transform.position) >= range)
            {
                Destroy(gameObject);
            }
        }
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            // Check for enemy collision
            if (other.gameObject.layer == enemyMask)
            {
                //todo: Damage
                Destroy(gameObject);
            }

            if (other.gameObject.layer == environmentMask)
            {
                Destroy(gameObject);
            }
        }


    }
}

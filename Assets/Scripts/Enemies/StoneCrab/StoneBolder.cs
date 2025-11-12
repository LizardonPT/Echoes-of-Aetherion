using FMODUnity;
using UnityEngine;

namespace EchoesOfEtherion.Enemies.StoneScorpion
{
    public class StoneBolder : MonoBehaviour
    {
        [SerializeField] private EventReference rockHit;

        [SerializeField] private LayerMask playerMask;
        [SerializeField] private LayerMask environmentMask;
        [SerializeField] private float radius;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float collisionCheckThreshold = 32f;

        [SerializeField] private Transform bolder;
        [SerializeField] private float maxHeight;
        [SerializeField] private float duration;

        private float height = 0;
        private float timer = 0;
        private bool hasReachedThreshold = false;
        private Vector3 initialPosition;
        private Vector3 targetPosition;
        private Vector2 startPos;

        private void Awake()
        {
            if (bolder == null)
                bolder = transform;

            initialPosition = bolder.localPosition;
        }

        public void Initialize(Vector3 startPos, Vector3 targetPos, float damageAmount = 20f)
        {
            targetPosition = targetPos;
            damage = damageAmount;

            timer = 0;
            height = 0;
            hasReachedThreshold = false;
            bolder.localPosition = initialPosition;
            this.startPos = startPos;
        }

        private void Update()
        {
            if (timer >= duration)
            {
                Destroy(gameObject);
                return;
            }

            timer += Time.deltaTime;
            float progress = timer / duration;

            height = Mathf.Sin(progress * Mathf.PI) * maxHeight;

            Vector3 newPos = bolder.localPosition;
            newPos.y = initialPosition.y + height;
            bolder.localPosition = newPos;

            if (!hasReachedThreshold && height >= collisionCheckThreshold)
            {
                hasReachedThreshold = true;
            }

            if (hasReachedThreshold)
            {
                CheckForCollisions();
            }

            transform.position = new(
                Mathf.Lerp(startPos.x, targetPosition.x, progress),
                Mathf.Lerp(startPos.y, targetPosition.y, progress),
                0);
            
            if (progress >= 1)
            {
                RuntimeManager.PlayOneShot(rockHit, transform.position);
                Destroy(gameObject);
            }
        }

        private void CheckForCollisions()
        {
            // Get world position for collision detection
            Vector3 worldPosition = bolder.position;

            // Check for player collisions
            Collider2D[] playerHits = Physics2D.OverlapCircleAll(worldPosition, radius, playerMask);
            foreach (Collider2D hit in playerHits)
            {
                if (hit.TryGetComponent<HealthSystem>(out var playerHealth))
                {
                    playerHealth.Damage(damage);
                    OnHitTarget();
                    return;
                }
            }

            // Check for environment collisions (walls, ground, etc.)
            Collider2D[] environmentHits = Physics2D.OverlapCircleAll(worldPosition, radius, environmentMask);

            if (environmentHits.Length > 0)
            {
                OnHitEnvironment();
            }
        }

        private void OnHitTarget()
        {
            RuntimeManager.PlayOneShot(rockHit, transform.position);

            Destroy(gameObject);
        }

        private void OnHitEnvironment()
        {
            RuntimeManager.PlayOneShot(rockHit, transform.position);

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (bolder != null)
            {
                Gizmos.color = hasReachedThreshold ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(bolder.position, radius);
            }
        }
#endif
    }
}

using System.Collections.Generic;
using System.Linq;
using EchoesOfEtherion.Game.Interactions;
using Unity.VisualScripting;
using UnityEngine;

namespace EchoesOfEtherion.Player.Components
{
    public class PlayerInteractor : MonoBehaviour
    {
        [field: Header("Interaction Settings")]
        [field: SerializeField] public float InteractRange { get; private set; } = 120f;
        [SerializeField] private LayerMask interactLayers;
        [SerializeField] private int maxResults = 5;

        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;
#if UNITY_EDITOR
        [SerializeField] private bool showInteractionRange = false;
#endif

        private Collider2D[] hitBuffer;
        private ContactFilter2D contactFilter;

        private void Awake()
        {
            hitBuffer = new Collider2D[Mathf.Max(1, maxResults)];

            contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(interactLayers);
        }

        public void InteractInput()
        {
            Log("Interact input received.");

            int hitCount = Physics2D.OverlapCircle(transform.position, InteractRange, contactFilter, hitBuffer);

            IInteractable nearest = null;
            float nearestDistanceSq = float.MaxValue;
            Vector2 myPos = transform.position;

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = hitBuffer[i];
                if (col == null) continue;

                if (col.TryGetComponent<IInteractable>(out var interactable))
                {
                    float dSq = (col.transform.position - (Vector3)myPos).sqrMagnitude;
                    if (dSq < nearestDistanceSq)
                    {
                        nearest = interactable;
                        nearestDistanceSq = dSq;
                    }
                }
            }

            if (nearest != null)
            {
                Log($"Interacting with {((Component)nearest).name}.");
                nearest.Interact();
                return;
            }

            Log("No interactable object found in range.");
        }


        private void Log(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[PlayerInteractor] {message}");
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showInteractionRange)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, InteractRange);
            }
        }
#endif
    }
}

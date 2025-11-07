using EchoesOfEtherion.Game.Interactions;
using UnityEngine;

namespace EchoesOfEtherion.Game.NPCs
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        public void Interact()
        {
            Log("NPC interacted with.");
        }

        private void Log(string message)
        {
            if (enableLogging)
            {
                Debug.Log($"[NPC | {name}]: {message}");
            }
        }
    }
}

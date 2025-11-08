using EchoesOfEtherion.Game.Interactions;
using EchoesOfEtherion.QuestSystem;
using UnityEngine;

namespace EchoesOfEtherion.Game.NPCs
{
    [RequireComponent(typeof(QuestPoint))]
    public class NPC : MonoBehaviour, IInteractable
    {
        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        private QuestPoint questPoint;

        private void Awake()
        {
            questPoint ??= GetComponent<QuestPoint>();
        }

        public void Interact()
        {
            Log("NPC interacted with.");
            questPoint.UpdateQuest();
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

using EchoesOfEtherion.Game.Interactions;
using EchoesOfEtherion.Player.Components;
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

        [SerializeField] private GameObject interactButton;

        private PlayerInteractor playerInteractor;

        private void Awake()
        {
            questPoint ??= GetComponent<QuestPoint>();
        }

        private void Start()
        {
            playerInteractor = FindAnyObjectByType<PlayerInteractor>();
        }

        private void Update()
        {
            if (questPoint.CurrentState == QuestState.CanStart ||
                questPoint.CurrentState == QuestState.CanFinish)
            {
                if (Vector2.Distance(playerInteractor.transform.position,
                                transform.position) <= playerInteractor.InteractRange)
                {
                    interactButton.SetActive(true);
                }
                else interactButton.SetActive(false);
            }
            else interactButton.SetActive(false);
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

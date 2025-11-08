using UnityEngine;

namespace EchoesOfEtherion.QuestSystem
{
    public class QuestIcon : MonoBehaviour
    {
        [SerializeField] private Sprite exclamationMarkIcon;
        [SerializeField] private Sprite questionMarkIcon;
        [SerializeField] private Color grey = Color.grey;
        [SerializeField] private Color yellow = Color.yellow;

        [SerializeField] private SpriteRenderer sr;

        public void UpdateState(QuestState state, bool startPoint, bool finishPoint)
        {
            switch (state)
            {
                case QuestState.RequirementsNotMet:
                    if (startPoint)
                        SetIcon(exclamationMarkIcon, grey);
                    break;
                case QuestState.CanStart:
                    if (startPoint)
                        SetIcon(exclamationMarkIcon, yellow);
                    break;
                case QuestState.InProgress:
                        SetIcon(questionMarkIcon, grey);
                    break;
                case QuestState.CanFinish:
                    if (finishPoint)
                        SetIcon(questionMarkIcon, yellow);
                    break;
                case QuestState.Finished:
                    sr.sprite = null;
                    sr.enabled = false;
                    break;
                default:
                    if (startPoint)
                        SetIcon(exclamationMarkIcon, grey);
                    break;
            }
        }

        private void SetIcon(Sprite icon, Color colour)
        {
            sr.sprite = icon;
            sr.color = colour;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sr == null)
                Debug.LogWarning($"[QuestIcon | {name}]: SpriteRenderer reference is missing.");
        }
#endif
    }
}
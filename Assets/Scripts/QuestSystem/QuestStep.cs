using UnityEngine;

namespace EchoesOfEtherion.QuestSystem.QuestSteps
{
    public abstract class QuestStep : MonoBehaviour
    {
        private bool isFinished = false;
        private int id;

        public void InitializeQuestStep(int id)
        {
            this.id = id;
        }

        protected void FinishQuestStep()
        {
            if (isFinished)
                return;

            isFinished = true;

            QuestManager.Instance.QuestEvents.OnAdvanceQuestStep(id);

            Destroy(gameObject);
        }
    }
}

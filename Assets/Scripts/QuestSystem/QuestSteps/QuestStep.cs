using System;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem.QuestSteps
{
    public abstract class QuestStep : MonoBehaviour
    {
        protected bool isFinished = false;
        protected int id;

        public abstract string StepDescription { get; protected set; }

        public abstract event Action<int, int> ProgressChanged;

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

        public abstract (int, int) GetProgress();
    }
}

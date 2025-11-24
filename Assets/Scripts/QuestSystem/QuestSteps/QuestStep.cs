using System;
using UnityEngine;
using EchoesOfEtherion.CurrencySystem;

namespace EchoesOfEtherion.QuestSystem.QuestSteps
{
    public abstract class QuestStep : MonoBehaviour
    {
        protected bool isFinished = false;
        protected string id;

        public abstract string StepDescription { get; protected set; }
        public abstract int GoldReward { get; protected set; }

        public abstract event Action<int, int> ProgressChanged;

        private GoldModule playerGold;

        public void InitializeQuestStep(string id)
        {
            this.id = id;
        }

        protected void FinishQuestStep()
        {
            if (isFinished)
                return;

            isFinished = true;

            QuestManager.Instance.QuestEvents.OnAdvanceQuestStep(id);

            playerGold?.AddGold(GoldReward);

            Destroy(gameObject);
        }

        public abstract (int, int) GetProgress();
    }
}

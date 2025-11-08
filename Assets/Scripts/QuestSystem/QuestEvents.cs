using System;

namespace EchoesOfEtherion.QuestSystem
{
    public class QuestEvents
    {
        public event Action<int> StartQuest;
        public event Action<int> FinishQuest;
        public event Action<int> AdvanceQuestStep;
        public event Action<Quest> QuestStateChanged;

        public void OnStartQuest(int questId)
        {
            StartQuest?.Invoke(questId);
        }

        public void OnFinishQuest(int questId)
        {
            FinishQuest?.Invoke(questId);
        }

        public void OnAdvanceQuestStep(int questId)
        {
            AdvanceQuestStep?.Invoke(questId);
        }

        public void OnQuestStateChanged(Quest quest)
        {
            QuestStateChanged?.Invoke(quest);
        }
    }
}
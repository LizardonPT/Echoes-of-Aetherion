using System;

namespace EchoesOfEtherion.QuestSystem
{
    public class QuestEvents
    {
        public event Action<string> StartQuest;
        public event Action<string> FinishQuest;
        public event Action<string> AdvanceQuestStep;
        public event Action<Quest> QuestStateChanged;

        public void OnStartQuest(string questId)
        {
            StartQuest?.Invoke(questId);
        }

        public void OnFinishQuest(string questId)
        {
            FinishQuest?.Invoke(questId);
        }

        public void OnAdvanceQuestStep(string questId)
        {
            AdvanceQuestStep?.Invoke(questId);
        }

        public void OnQuestStateChanged(Quest quest)
        {
            QuestStateChanged?.Invoke(quest);
        }
    }
}
using System;
using System.Linq;
using UnityEngine;

namespace EchoesOfEtherion.QuestSystem
{
    [CreateAssetMenu(
        fileName = "QuestInfoSO",
        menuName = "Scriptable Objects/Quest System/Quest Info")]
    public class QuestInfoSO : ScriptableObject
    {
        [field: Header("General Info")]
        [field: SerializeField]
        public string ID { get; private set; }
        [field: SerializeField]
        public string DisplayName { get; private set; } = string.Empty;

        [field: SerializeField, TextArea]
        public string Description { get; private set; } = string.Empty;
        [field: SerializeField]
        public string CompleteText { get; private set; } = string.Empty;

        [field: Header("Requirements")]
        [field: SerializeField, Min(1)]
        public uint RequiredLevel { get; private set; } = 1;

        [SerializeField]
        private QuestInfoSO[] questPrerequisites = Array.Empty<QuestInfoSO>();

        [field: Header("Steps")]
        [field: SerializeField]
        public GameObject[] QuestStepPrefabs { get; private set; } = Array.Empty<GameObject>();

        [field: Header("Rewards")]
        [field: SerializeField]
        public uint ExperienceReward { get; private set; } = 0;

        [field: SerializeField]
        public uint GoldReward { get; private set; } = 0;

        /// <summary>
        /// Returns a shallow copy of the prerequisite array to prevent external modification.
        /// </summary>
        public QuestInfoSO[] QuestPrerequisites => questPrerequisites?.ToArray() ?? Array.Empty<QuestInfoSO>();


#if UNITY_EDITOR
        private void OnValidate()
        {
            // Automatically fix null arrays to avoid runtime issues.
            QuestStepPrefabs ??= Array.Empty<GameObject>();
            questPrerequisites ??= Array.Empty<QuestInfoSO>();

            bool displayNameCorrect = !string.IsNullOrWhiteSpace(DisplayName);
            bool idNotFilled = string.IsNullOrWhiteSpace(ID);
            if (displayNameCorrect && idNotFilled)
            {
                ID = DisplayName.ToLower().Replace(' ', '_');
            }
        }
#endif
    }
}
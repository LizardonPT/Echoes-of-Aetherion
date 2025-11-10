#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace EchoesOfEtherion.QuestSystem.Editor
{
    public static class QuestIdValidator
    {
        [MenuItem("Tools/Quest System/Validate Quest IDs")]
        public static void ValidateQuestIds()
        {
            var quests = Resources.LoadAll<QuestInfoSO>("Quests");

            var idMap = new Dictionary<string, List<string>>();
            bool hasError = false;

            foreach (var quest in quests)
            {
                string id = quest.ID?.Trim();
                string path = AssetDatabase.GetAssetPath(quest);

                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogError($"Quest '{quest.name}' at '{path}' has an empty or invalid ID!");
                    hasError = true;
                    continue;
                }

                if (!idMap.TryGetValue(id, out var list))
                {
                    list = new List<string>();
                    idMap[id] = list;
                }

                list.Add(path);
            }

            foreach (var pair in idMap.Where(p => p.Value.Count > 1))
            {
                hasError = true;
                string joinedPaths = string.Join("\n- ", pair.Value);
                Debug.LogError(
                    $"Duplicate Quest ID found: '{pair.Key}'\n" +
                    $"- {joinedPaths}"
                );
            }

            if (!hasError)
                Debug.Log($"All {quests.Length} quest IDs are unique!");
        }
    }
}
#endif

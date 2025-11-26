using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.DeveloperConsole
{
    [RequireComponent(typeof(ConsoleController))]
    public class ConsoleLogger : Singleton<ConsoleLogger>
    {
        [Header("References")]
        [SerializeField] private TMP_InputField logTMP;

        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float padding = 6;

        [Header("Console Settings")]
        [SerializeField] private int maxLines = 100;

        private readonly List<string> lines = new();
        private bool userScrolled = false;
        private RectTransform logTMPRectTransform;

        protected override void Awake()
        {
            base.Awake();
            logTMPRectTransform = logTMP.GetComponent<RectTransform>();

            Log("Developer consoler initiated.");
        }

        private void Start()
        {
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        public static void Log(string message)
        {
            if (Instance == null) return;
            Instance.AppendLine(message);
        }

        private void AppendLine(string message)
        {
            lines.Add(message);

            while (lines.Count > maxLines)
            {
                lines.RemoveAt(0);
            }

            logTMP.text = string.Join("\n", lines);

            ScaleText();
            FollowLatestLog();
        }

        private static void ScaleText()
        {
            if (Instance == null) return;

            Instance.logTMP.textComponent.ForceMeshUpdate();

            float width = Instance.logTMP.textComponent.preferredWidth;
            float height = Instance.logTMP.textComponent.preferredHeight;
            if (Instance.logTMPRectTransform == null)
                Instance.logTMPRectTransform = Instance.logTMP.GetComponent<RectTransform>();

            if (Instance.logTMPRectTransform != null)
                Instance.logTMPRectTransform.sizeDelta = new Vector2(width + Instance.padding, height + Instance.padding);
        }


        private void FollowLatestLog()
        {
            if (!userScrolled)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        private void OnScrollValueChanged(Vector2 pos)
        {
            // If the user scrolls anywhere except the very bottom,
            // stop auto-follow
            userScrolled = scrollRect.verticalNormalizedPosition > 0.001f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (logTMP == null)
                Debug.LogWarning("[ConsoleLogger] logTMP is null.");

            padding = padding < 0 ? 0 : padding;
        }
#endif
    }
}
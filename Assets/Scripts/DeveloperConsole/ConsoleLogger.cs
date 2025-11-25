using TMPro;
using UnityEngine;

namespace EchoesOfEtherion.DeveloperConsole
{
    public class ConsoleLogger : Singleton<ConsoleLogger>
    {
        [SerializeField] private TextMeshProUGUI logTMP;
        [SerializeField] private float padding = 6;

        protected override void Awake()
        {
            base.Awake();
            Log("Developer consoler initiated.");
        }

        public static void Log(string message)
        {
            if (Instance == null) return;

            Instance.logTMP.text += $"{message}\n";

            ScaleText();
        }

        private static void ScaleText()
        {
            if (Instance == null) return;

            Instance.logTMP.ForceMeshUpdate();

            float width = Instance.logTMP.preferredWidth;
            float height = Instance.logTMP.preferredHeight;

            Instance.logTMP.rectTransform.sizeDelta = new Vector2(width + Instance.padding, height + Instance.padding);
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
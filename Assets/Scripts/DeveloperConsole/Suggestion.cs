using System;
using EchoesOfEtherion.DeveloperConsole.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EchoesOfEtherion.DeveloperConsole
{
    [RequireComponent(typeof(Button))]
    public class Suggestion : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI SuggestionTMP { get; private set; }
        [SerializeField] private Image backgroundImage;

        public event Action<Suggestion> OnClick;
        private RectTransform parentRectTransform;
        private Button button;
        private RectTransform rectTransform;

        private IConsoleCommand consoleCommand;

        public void Initialize(IConsoleCommand command)
        {
            consoleCommand = command;
            SuggestionTMP.text = command.Usage;
            button = GetComponent<Button>();
            button.onClick.AddListener(Clicked);
        }

        public void Clicked()
        {
            OnClick?.Invoke(this);
        }

        public string GetCommand()
        {
            return consoleCommand.Key;
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        public void HighlightButton()
        {
            backgroundImage.color = button.colors.highlightedColor;
        }

        public void UnhighlightButton()
        {
            backgroundImage.color = Color.white;
        }

        private void LateUpdate()
        {
            rectTransform.sizeDelta = new Vector2(parentRectTransform.rect.width, rectTransform.sizeDelta.y);
        }
    }
}
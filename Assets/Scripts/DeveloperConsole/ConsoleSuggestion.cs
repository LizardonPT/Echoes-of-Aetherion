using System;
using System.Collections.Generic;
using EchoesOfEtherion.DeveloperConsole.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfEtherion.DeveloperConsole
{
    public class ConsoleSuggestion : MonoBehaviour
    {
        [SerializeField] private Suggestion suggestionPrefab;
        [SerializeField] private VerticalLayoutGroup suggestionContainer;
        [SerializeField] private ScrollRect suggestionScrollRect;
        [SerializeField] private float widthPadding = 20;
        [SerializeField] private float heightPadding = 50;

        public event Action<string> SuggestionClicked;

        private RectTransform suggestionContainerRectTransform;
        private RectTransform prefabRectTransform;

        readonly List<Suggestion> suggestions = new();

        private int currentIndex = -1;

        private IEnumerable<IConsoleCommand> availableCommands;

        public int SuggestionCount => suggestions.Count;

        private void Awake()
        {
            suggestionContainerRectTransform = suggestionContainer.GetComponent<RectTransform>();
            prefabRectTransform = suggestionPrefab.GetComponent<RectTransform>();
        }


        private void Start()
        {
            availableCommands = CommandDatabase.Instance.GetAllCommands();
            ClearSuggestions();
        }

        public void UpdateSuggestions(string newText)
        {
            ClearSuggestions();

            if (string.IsNullOrWhiteSpace(newText))
                return;

            newText = newText.Split(' ')[0];

            foreach (IConsoleCommand command in availableCommands)
            {
                if (command.Key.StartsWith(newText, System.StringComparison.OrdinalIgnoreCase))
                {
                    InstantiateSuggestion(command);
                }
            }

            if (suggestions.Count > 0)
                suggestionScrollRect.gameObject.SetActive(true);

            currentIndex = -1;
        }

        public void UpdateArrowDown()
        {
            currentIndex++;
            if (currentIndex >= suggestions.Count)
                currentIndex = suggestions.Count - 1;

            if (suggestions.Count == 0)
                return;

            foreach (var suggestion in suggestions)
            {
                suggestion.UnhighlightButton();
            }

            if (currentIndex < suggestions.Count)
            {
                Suggestion suggestion = suggestions[currentIndex];
                suggestion.HighlightButton();
            }
        }

        public void UpdateArrowUp()
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = 0;

            if (suggestions.Count == 0)
                return;

            foreach (var suggestion in suggestions)
            {
                suggestion.UnhighlightButton();
            }

            if (currentIndex < suggestions.Count)
            {
                Suggestion suggestion = suggestions[currentIndex];
                suggestion.HighlightButton();
            }
        }

        public bool TryGetCurrentSuggestion(out string suggestion)
        {
            if (suggestions.Count == 0)
            {
                suggestion = string.Empty;
                return false;
            }

            if (currentIndex >= 0 && currentIndex < suggestions.Count)
            {
                suggestion = suggestions[currentIndex].GetCommand();
                return true;
            }

            suggestion = string.Empty;
            return false;
        }

        private void InstantiateSuggestion(IConsoleCommand consoleCommand)
        {
            Suggestion newSuggestion = Instantiate(suggestionPrefab, suggestionContainer.transform);
            newSuggestion.Initialize(consoleCommand);
            newSuggestion.OnClick += OnSuggestionClicked;

            suggestions.Add(newSuggestion);
        }

        private void OnSuggestionClicked(Suggestion suggestion)
        {
            SuggestionClicked?.Invoke(suggestion.GetCommand());
        }

        private void LateUpdate()
        {
            if (suggestions.Count == 0)
                return;

            Vector2 size = GetSuggestionWishSize();

            size.x += widthPadding;

            float height = 0;
            height += prefabRectTransform.sizeDelta.y * suggestions.Count + suggestionContainer.spacing * (suggestions.Count - 1);

            size.y = height + heightPadding;

            float x = size.x > suggestionScrollRect.viewport.rect.width ?
                size.x : suggestionScrollRect.viewport.rect.width;

            suggestionContainerRectTransform.sizeDelta = new Vector2(x, size.y);
        }

        private Vector2 GetSuggestionWishSize()
        {
            if (suggestions.Count == 0)
                return Vector2.zero;

            Vector2 biggest = Vector2.zero;

            foreach (var suggestion in suggestions)
            {
                Vector2 size = suggestion.SuggestionTMP.GetPreferredValues();
                if (size.x > biggest.x)
                    biggest.x = size.x;
                if (size.y > biggest.y)
                    biggest.y = size.y;
            }

            return biggest;
        }

        public void ClearSuggestions()
        {
            foreach (var suggestion in suggestions)
            {
                Destroy(suggestion.gameObject);
            }

            suggestions.Clear();
            suggestionScrollRect.gameObject.SetActive(false);
            currentIndex = -1;
        }
    }
}
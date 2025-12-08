using System;
using System.Collections.Generic;
using System.Linq;
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

        private RectTransform containerRect;
        private RectTransform prefabRect;

        private readonly List<Suggestion> suggestions = new();
        private IEnumerable<IConsoleCommand> availableCommands;

        private int currentIndex = -1;
        public int SuggestionCount => suggestions.Count;

        private void Awake()
        {
            containerRect = suggestionContainer.GetComponent<RectTransform>();
            prefabRect = suggestionPrefab.GetComponent<RectTransform>();
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

            List<IConsoleCommand> perfectMatches = new();
            List<IConsoleCommand> partialMatches = new();

            foreach (IConsoleCommand command in availableCommands)
            {
                if (command.Key.StartsWith(newText, StringComparison.OrdinalIgnoreCase))
                {
                    perfectMatches.Add(command);
                }
                else if (command.Key.IndexOf(newText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    partialMatches.Add(command);
                }
            }

            foreach (var command in perfectMatches.Concat(partialMatches))
            {
                CreateSuggestion(command);
            }

            if (suggestions.Count > 0)
                suggestionScrollRect.gameObject.SetActive(true);

            currentIndex = 0;
            UpdateHighlight();
        }


        public void UpdateArrowDown()
        {
            if (suggestions.Count == 0)
                return;

            EnsureIndexWithinRange(currentIndex + 1);
            UpdateHighlight();
        }

        public void UpdateArrowUp()
        {
            if (suggestions.Count == 0)
                return;

            EnsureIndexWithinRange(currentIndex - 1);
            UpdateHighlight();
        }

        public bool TryGetCurrentSuggestion(out IConsoleCommand command)
        {
            if (currentIndex >= 0 && currentIndex < suggestions.Count)
            {
                command = suggestions[currentIndex].GetCommand();
                return true;
            }

            command = null;
            return false;
        }

        private void CreateSuggestion(IConsoleCommand command)
        {
            var newSuggestion = Instantiate(suggestionPrefab, suggestionContainer.transform);
            newSuggestion.Initialize(command);
            newSuggestion.OnClick += OnSuggestionInternalClick;

            suggestions.Add(newSuggestion);
        }

        private void OnSuggestionInternalClick(Suggestion suggestion)
        {
            SuggestionClicked?.Invoke(suggestion.GetCommand().Key);
        }

        private void EnsureIndexWithinRange(int index)
        {
            if (suggestions.Count == 0)
            {
                currentIndex = -1;
                return;
            }

            currentIndex = Mathf.Clamp(index, 0, suggestions.Count - 1);
        }

        private void UpdateHighlight()
        {
            if (suggestions.Count == 0)
                return;

            for (int i = 0; i < suggestions.Count; i++)
            {
                if (i == currentIndex)
                    suggestions[i].HighlightButton();
                else
                    suggestions[i].UnhighlightButton();
            }

            ScrollToIndex(currentIndex);
        }

        private void LateUpdate()
        {
            if (suggestions.Count == 0)
                return;

            Vector2 largestTextSize = GetLargestSuggestionTextSize();

            float preferredWidth = largestTextSize.x + widthPadding;
            float preferredHeight =
                (prefabRect.sizeDelta.y * suggestions.Count) +
                (suggestionContainer.spacing * (suggestions.Count - 1)) +
                heightPadding;

            float finalWidth = Mathf.Max(preferredWidth, suggestionScrollRect.viewport.rect.width);

            containerRect.sizeDelta = new Vector2(finalWidth, preferredHeight);
        }

        private void ScrollToIndex(int index)
        {
            if (suggestions.Count <= 1)
                return;

            float t = (float)index / (suggestions.Count - 1);
            suggestionScrollRect.verticalNormalizedPosition = 1f - t;
        }

        private Vector2 GetLargestSuggestionTextSize()
        {
            Vector2 biggest = Vector2.zero;

            for (int i = 0; i < suggestions.Count; i++)
            {
                Vector2 size = suggestions[i].SuggestionTMP.GetPreferredValues();

                if (size.x > biggest.x)
                    biggest.x = size.x;
                if (size.y > biggest.y)
                    biggest.y = size.y;
            }

            return biggest;
        }

        public void ClearSuggestions()
        {
            for (int i = 0; i < suggestions.Count; i++)
            {
                suggestions[i].OnClick -= OnSuggestionInternalClick;
                Destroy(suggestions[i].gameObject);
            }

            suggestions.Clear();
            suggestionScrollRect.gameObject.SetActive(false);
            currentIndex = -1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using EchoesOfEtherion.DeveloperConsole.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.DeveloperConsole
{
    [RequireComponent(typeof(ConsoleController), typeof(ConsoleSuggestion))]
    public class ConsoleInputField : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private int maxHistory = 100;

        private ConsoleController controller;
        private ConsoleSuggestion suggestionUI;

        private readonly List<string> commandHistory = new();
        private int historyIndex = 0;

        private bool onHistory = false;
        private bool HasSuggestions => suggestionUI.SuggestionCount > 0;

        private void Awake()
        {
            controller = GetComponent<ConsoleController>();
            suggestionUI = GetComponent<ConsoleSuggestion>();
        }

        private void Start()
        {
            inputField.onSubmit.AddListener(OnSubmit);
            inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable()
        {
            controller.ConsoleOpened += OnConsoleOpened;
            controller.ConsoleClosed += OnConsoleClosed;
            suggestionUI.SuggestionClicked += OnSuggestionClicked;
        }

        private void OnDisable()
        {
            controller.ConsoleOpened -= OnConsoleOpened;
            controller.ConsoleClosed -= OnConsoleClosed;
            suggestionUI.SuggestionClicked -= OnSuggestionClicked;
        }

        private void Update()
        {
            if (!controller.IsOpen)
                return;

            HandleArrowNavigation();
            HandleTab();
        }

        private void HandleArrowNavigation()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.upArrowKey.wasPressedThisFrame)
            {
                MoveCursorToEnd();

                if (HasSuggestions)
                {
                    suggestionUI.UpdateArrowUp();
                    return;
                }

                NavigateHistory(-1);
            }
            else if (keyboard.downArrowKey.wasPressedThisFrame)
            {
                MoveCursorToEnd();

                if (HasSuggestions)
                {
                    suggestionUI.UpdateArrowDown();
                    return;
                }

                NavigateHistory(1);
            }
        }

        private void HandleTab()
        {
            if (!Keyboard.current.tabKey.wasPressedThisFrame)
                return;

            if (HasSuggestions &&
                suggestionUI.TryGetCurrentSuggestion(out IConsoleCommand suggestion))
            {
                inputField.text = suggestion.Key + " ";
                MoveCursorToEnd();
            }
        }

        private void NavigateHistory(int direction)
        {
            if (commandHistory.Count == 0)
                return;

            if (!onHistory || string.IsNullOrEmpty(inputField.text))
            {
                onHistory = true;
                historyIndex = (direction < 0) ?
                    commandHistory.Count - 1 :
                    0;

                inputField.text = commandHistory[historyIndex];
                MoveCursorToEnd();
                return;
            }

            historyIndex += direction;

            if (historyIndex < 0)
                historyIndex = commandHistory.Count - 1;
            else if (historyIndex > commandHistory.Count - 1)
                historyIndex = 0;

            inputField.text = (historyIndex == 0 && direction > 0)
                ? ""
                : commandHistory[historyIndex];

            MoveCursorToEnd();
        }

        private void OnValueChanged(string value)
        {
            if (onHistory)
                return;

            if (string.IsNullOrEmpty(value))
            {
                suggestionUI.ClearSuggestions();
                return;
            }

            suggestionUI.UpdateSuggestions(value);
        }

        private void OnSubmit(string message)
        {
            if (HasSuggestions &&
                suggestionUI.TryGetCurrentSuggestion(out IConsoleCommand suggestion))
            {
                bool hasArgs = suggestion.ExpectedArguments.Count > 0;
                bool alreadyAccepted = message.Split(' ')[0] == suggestion.Key;

                if (!alreadyAccepted && hasArgs)
                {
                    inputField.text = suggestion.Key + " ";
                    MoveCursorToEnd();
                    return;
                }
                else if (!alreadyAccepted && !hasArgs)
                    message = suggestion.Key;
            }


            if (string.IsNullOrEmpty(message))
            {
                inputField.ActivateInputField();
                return;
            }

            ConsoleLogger.Log("");
            ConsoleLogger.Log($"> {message}");

            bool success = CommandDatabase.Instance.ExecuteCommand(message);

            if (!success)
                ConsoleLogger.Log("Command failed or not recognized");

            AddToHistory(message);

            inputField.text = "";
            onHistory = false;
            MoveCursorToEnd();
        }

        private void AddToHistory(string cmd)
        {
            if (commandHistory.Count >= maxHistory)
                commandHistory.RemoveAt(0);

            commandHistory.Add(cmd);
            historyIndex = 0;
        }

        private void MoveCursorToEnd()
        {
            inputField.caretPosition = inputField.text.Length;
            inputField.ActivateInputField();
        }

        private void OnSuggestionClicked(string suggestion)
        {
            inputField.text = suggestion;
            MoveCursorToEnd();
        }

        private void OnConsoleOpened() => inputField.ActivateInputField();
        private void OnConsoleClosed() => inputField.DeactivateInputField();
    }
}

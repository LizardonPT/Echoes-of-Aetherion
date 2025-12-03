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
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private int maxHistory = 100;

        private ConsoleController consoleController;
        private ConsoleSuggestion consoleSuggestion;

        private List<string> commandHistory = new();

        private int historyIndex = 0;

        private int HistoryIndex
        {
            get => historyIndex;
            set
            {
                if (value > commandHistory.Count - 1)
                {
                    historyIndex = 0;
                }
                else if (value < 0)
                {
                    historyIndex = commandHistory.Count - 1;
                }
                else historyIndex = value;
            }
        }

        private bool isOnHistory = false;
        private bool hasSuggestions;

        private void Awake()
        {
            consoleController = GetComponent<ConsoleController>();
            consoleSuggestion = GetComponent<ConsoleSuggestion>();
        }

        private void Start()
        {
            inputField.onSubmit.AddListener(OnSubmit);
            inputField.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnEnable()
        {
            consoleController.ConsoleOpened += OnConsoleOpened;
            consoleController.ConsoleClosed += OnConsoleClosed;
            consoleSuggestion.SuggestionClicked += OnSuggestionClicked;
        }

        private void OnDisable()
        {
            consoleController.ConsoleOpened -= OnConsoleOpened;
            consoleController.ConsoleClosed -= OnConsoleClosed;
            consoleSuggestion.SuggestionClicked -= OnSuggestionClicked;
        }

        private void Update()
        {
            if (!consoleController.IsOpen)
                return;

            hasSuggestions = consoleSuggestion.SuggestionCount > 0;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                if (hasSuggestions)
                { // Prioritize suggestions over history navigation
                    consoleSuggestion.UpdateArrowUp();
                    return;
                }

                if (!isOnHistory || inputField.text == "")
                {
                    isOnHistory = true;
                    HistoryIndex = commandHistory.Count - 1;
                }
                else HistoryIndex -= 1;

                if (commandHistory.Count > 0)
                {
                    inputField.text = commandHistory[HistoryIndex];
                    inputField.caretPosition = inputField.text.Count();
                }
            }
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                if (hasSuggestions)
                { // Prioritize suggestions over history navigation
                    consoleSuggestion.UpdateArrowDown();
                    return;
                }
                if (commandHistory.Count == 0) return;
                if (!isOnHistory || inputField.text == "")
                {
                    isOnHistory = true;
                    HistoryIndex = 0;
                    inputField.text = commandHistory[HistoryIndex];
                }
                else
                {
                    HistoryIndex++;
                    if (HistoryIndex == 0)
                        inputField.text = "";
                    else
                        inputField.text = commandHistory[HistoryIndex];
                }

                inputField.caretPosition = inputField.text.Count();
            }
            else if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (hasSuggestions)
                {
                    if (consoleSuggestion.TryGetCurrentSuggestion(out string suggestion))
                    {
                        inputField.text = suggestion + " ";
                        inputField.caretPosition = inputField.text.Count();
                        inputField.ActivateInputField();
                    }
                }
            }
        }

        private void OnValueChanged(string newText)
        {
            if (isOnHistory)
                return;

            if (string.IsNullOrEmpty(newText))
            {
                consoleSuggestion.ClearSuggestions();
                return;
            }

            consoleSuggestion.UpdateSuggestions(newText);
        }

        private void OnSubmit(string message)
        {
            if (hasSuggestions)
            {
                if (consoleSuggestion.TryGetCurrentSuggestion(out string suggestion))
                {
                    inputField.text = suggestion + " ";
                    inputField.caretPosition = inputField.text.Count();
                    inputField.ActivateInputField();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                // Log the command that was entered
                ConsoleLogger.Log($"");
                ConsoleLogger.Log($"> {message}");

                // Execute the command through the database
                bool success = CommandDatabase.Instance.ExecuteCommand(message);

                if (!success)
                {
                    ConsoleLogger.Log($"Command failed or not recognized");
                }

                // Add to history
                if (commandHistory.Count == maxHistory)
                {
                    commandHistory.RemoveAt(0);
                }
                commandHistory.Add(message);
                isOnHistory = false;
                HistoryIndex = 0;

                inputField.text = "";
                inputField.ActivateInputField();
            }
            else
                inputField.ActivateInputField();
        }

        private void OnSuggestionClicked(string suggestion)
        {
            inputField.text = suggestion;
            inputField.caretPosition = inputField.text.Count();
            inputField.ActivateInputField();
        }

        private void OnConsoleOpened()
        {
            inputField.ActivateInputField();
        }

        private void OnConsoleClosed()
        {
            inputField.DeactivateInputField();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using EchoesOfEtherion.DeveloperConsole.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoesOfEtherion.DeveloperConsole
{
    [RequireComponent(typeof(ConsoleController))]
    public class ConsoleInputField : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private int maxHistory = 100;

        private ConsoleController consoleController;

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

        private void Awake()
        {
            consoleController = GetComponent<ConsoleController>();
        }

        private void Start()
        {
            inputField.onSubmit.AddListener(OnSubmit);
        }

        private void OnEnable()
        {
            consoleController.ConsoleOpened += OnConsoleOpened;
            consoleController.ConsoleClosed += OnConsoleClosed;
        }

        private void OnDisable()
        {
            consoleController.ConsoleOpened -= OnConsoleOpened;
            consoleController.ConsoleClosed -= OnConsoleClosed;
        }

        private void Update()
        {
            if (!consoleController.IsOpen)
                return;
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
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

        }

        private void OnSubmit(string message)
        {
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

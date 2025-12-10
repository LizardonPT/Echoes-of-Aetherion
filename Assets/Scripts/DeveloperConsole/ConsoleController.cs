using System;
using System.Linq;
using EchoesOfEtherion.DeveloperConsole.CFG;
using EchoesOfEtherion.DeveloperConsole.Commands;
using EchoesOfEtherion.DeveloperConsole.Inputs;
using EchoesOfEtherion.Game.Helpers;
using EchoesOfEtherion.Game.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace EchoesOfEtherion.DeveloperConsole
{
    [RequireComponent(typeof(ConsoleLogger))]
    [RequireComponent(typeof(CommandDatabase))]
    [RequireComponent(typeof(CFGReaderWriter))]
    [RequireComponent(typeof(InputBindingManager))]
    public class ConsoleController : Singleton<ConsoleController>
    {
        [Header("References")]
        [SerializeField] private InputActionReference openCloseActionReference;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;

        public event Action ConsoleOpened;
        public event Action ConsoleClosed;

        public bool IsOpen { get; private set; }

        public ConsoleLogger Logger
        {
            get
            {
                if (logger == null)
                    logger = GetComponent<ConsoleLogger>();

                return logger ?? null;
            }
        }

        [SerializeField] private ConsoleLogger logger;
        
        private CFGReaderWriter cFGReaderWriter;
        private CommandDatabase commandDB;
        private InputBindingManager inputBindingManager;

        public CommandDatabase CommandDatabase
        {
            get
            {
                commandDB ??= GetComponent<CommandDatabase>();
                return commandDB ?? null;
            }
        }

        public CFGReaderWriter CFGReaderWriter
        {
            get
            {
                cFGReaderWriter ??= GetComponent<CFGReaderWriter>();
                return cFGReaderWriter ?? null;
            }
        }

        public InputBindingManager InputBindingManager
        {
            get
            {
                inputBindingManager ??= GetComponent<InputBindingManager>();
                return inputBindingManager;
            }
        }
        

        private void Start()
        {
            panel.SetActive(false);
            ConsoleClosed?.Invoke();
            IsOpen = false;
            closeButton.onClick.AddListener(() => { if (IsOpen) ToggleCloseConsole(); });
        }

        private void OnEnable()
        {
            openCloseActionReference.action.performed += OnOpenCloseActionPerformed;
        }

        private void OnDisable()
        {
            openCloseActionReference.action.performed -= OnOpenCloseActionPerformed;
        }

        private void OnOpenCloseActionPerformed(InputAction.CallbackContext context)
        {
            ToggleCloseConsole();
        }

        private void ToggleCloseConsole()
        {
            if (IsOpen)
            {
                panel.SetActive(false);
                ConsoleClosed?.Invoke();
                IsOpen = false;
            }
            else
            {
                panel.SetActive(true);
                ConsoleOpened?.Invoke();
                IsOpen = true;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (openCloseActionReference == null)
                Debug.LogError("[ConsoleController] openCloseActionReference is null.");

            logger ??= GetComponent<ConsoleLogger>();

            if (panel == null)
            {
                GameObject panelTest = null;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    if (child.name == "Panel")
                    {
                        panelTest = child.gameObject;
                        break;
                    }
                }

                if (panelTest != null)
                    panel = panelTest;
                else
                    Debug.LogError("[ConsoleController] Panel is null and couldn't find one.");
            }
        }
#endif
    }
}

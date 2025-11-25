using System;
using System.Linq;
using EchoesOfEtherion.Game.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace EchoesOfEtherion.DeveloperConsole
{
    public class ConsoleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputActionReference openCloseActionReference;
        [SerializeField] private GameObject panel;
        [SerializeField] private Button closeButton;

        public event Action OpenConsole;
        public event Action CloseConsole;

        public bool IsOpen { get; private set; }

        private void Start()
        {
            panel.SetActive(false);
            CloseConsole?.Invoke();
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
                CloseConsole?.Invoke();
                IsOpen = false;
            }
            else
            {
                panel.SetActive(true);
                OpenConsole?.Invoke();
                IsOpen = true;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (openCloseActionReference == null)
                Debug.LogError("[ConsoleController] openCloseActionReference is null.");

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

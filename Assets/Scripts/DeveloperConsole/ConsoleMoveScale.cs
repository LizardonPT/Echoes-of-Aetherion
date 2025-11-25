using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Linq;

namespace EchoesOfEtherion.DeveloperConsole
{
    [RequireComponent(typeof(ConsoleController))]
    public class ConsoleMoveScale : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject moveButton;
        [SerializeField] private GameObject scaleButton;

        [Header("Limits")]
        [SerializeField] private Vector2 minSize = new(300, 500);

        private RectTransform canvasRect;

        private bool isMoving = false;
        private bool isResizing = false;

        private Vector2 moveOffset;
        private Vector2 startMouse;
        private Vector2 startSize;

        private bool active = false;

        private ConsoleController controller;

        private void Awake()
        {
            controller = GetComponent<ConsoleController>();

            if (canvas == null) canvas = GetComponentInParent<Canvas>();

            if (canvas == null)
                Debug.LogError("[ConsoleMoveScale] No canvas found.");

            canvasRect = canvas.GetComponent<RectTransform>();

            AddEventTrigger(moveButton, EventTriggerType.PointerDown, (data) => StartMove());
            AddEventTrigger(moveButton, EventTriggerType.PointerUp, (data) => StopActions());

            AddEventTrigger(scaleButton, EventTriggerType.PointerDown, (data) => StartResize());
            AddEventTrigger(scaleButton, EventTriggerType.PointerUp, (data) => StopActions());
        }

        private void OnEnable()
        {
            controller.OpenConsole += OnOpenConsole;
            controller.CloseConsole += OnCloseConsole;
        }

        private void OnDisable()
        {
            controller.OpenConsole -= OnOpenConsole;
            controller.CloseConsole -= OnCloseConsole;
        }

        private void Update()
        {
            if (!Mouse.current.leftButton.isPressed || !active)
            {
                StopActions();
                return;
            }

            if (isMoving) MovePanel();
            else if (isResizing) ResizePanel();
        }

        private void StopActions()
        {
            isMoving = false;
            isResizing = false;
        }

        private void StartMove()
        {
            isMoving = true;
            isResizing = false;

            Vector2 mouse = Mouse.current.position.ReadValue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouse,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localMouse);

            moveOffset = localMouse - panel.anchoredPosition;
        }

        private void MovePanel()
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouse,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localMouse);

            Vector2 newPos = localMouse - moveOffset;

            // Clamp so panel stays inside canvas
            Vector2 size = panel.sizeDelta;
            Vector2 canvasSize = canvasRect.rect.size;

            newPos.x = Mathf.Clamp(newPos.x, 0, canvasSize.x - size.x);
            newPos.y = Mathf.Clamp(newPos.y, -canvasSize.y + size.y, 0);

            panel.anchoredPosition = newPos;
        }

        private void StartResize()
        {
            isResizing = true;
            isMoving = false;

            startMouse = Mouse.current.position.ReadValue();
            startSize = panel.sizeDelta;
        }

        private void ResizePanel()
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            Vector2 delta = mouse - startMouse;

            float newWidth = startSize.x + delta.x;
            float newHeight = startSize.y - delta.y;

            // Clamp to min size
            newWidth = Mathf.Max(newWidth, minSize.x);
            newHeight = Mathf.Max(newHeight, minSize.y);

            // Clamp to canvas bounds
            Vector2 panelPos = panel.anchoredPosition;
            Vector2 canvasSize = canvasRect.rect.size;

            float maxWidth = canvasSize.x - panelPos.x;
            float maxHeight = panelPos.y + canvasSize.y;
            newWidth = Mathf.Min(newWidth, maxWidth);
            newHeight = Mathf.Min(newHeight, maxHeight);

            panel.sizeDelta = new Vector2(newWidth, newHeight);
        }

        private void AddEventTrigger(GameObject obj, EventTriggerType type, System.Action<BaseEventData> action)
        {
            if (!obj.TryGetComponent<EventTrigger>(out var trigger))
                trigger = obj.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new() { eventID = type };
            entry.callback.AddListener((data) => action(data));
            trigger.triggers.Add(entry);
        }

        private void OnOpenConsole()
        {
            active = true;
        }

        private void OnCloseConsole()
        {
            active = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            canvas ??= GetComponentInChildren<Canvas>();

            if (panel == null)
            {
                RectTransform panelTest = null;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);

                    if (child.name == "Panel")
                    {
                        panelTest = child.GetComponent<RectTransform>();
                        break;
                    }
                }

                if (panelTest != null)
                    panel = panelTest;
                else
                    Debug.LogError("[ConsoleMoveScale] Panel is null and couldn't find one.");
            }

            minSize.x = minSize.x > 0 ? minSize.x : 0;
            minSize.y = minSize.y > 0 ? minSize.y : 0;
        }
#endif
    }
}

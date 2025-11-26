using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace EchoesOfEtherion.DeveloperConsole
{
    public class NonBlockingScrollRect : ScrollRect
    {
        // Don't do anything if the mouse clicks.
        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }

        /// <summary>
        /// Shift + scroll wheel will change horizontal slider.
        /// Normal scroll will change vertical slider.
        /// </summary>
        /// <param name="data">Data</param>
        public override void OnScroll(PointerEventData data)
        {
            bool shiftPressed =
                Keyboard.current != null &&
                (Keyboard.current.leftShiftKey.isPressed ||
                 Keyboard.current.rightShiftKey.isPressed);

            if (shiftPressed && horizontal)
            {
                float delta = -data.scrollDelta.y * 0.1f;

                horizontalNormalizedPosition =
                    Mathf.Clamp01(horizontalNormalizedPosition + delta);
            }
            else
            {
                base.OnScroll(data);
            }
        }
    }
}
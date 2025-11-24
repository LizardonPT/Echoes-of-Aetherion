using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public static class UIHelpers
{
    public static bool IsPointerOverInteractableUI()
    {
        Vector2 pointerPos;

        if (Pointer.current != null)
            pointerPos = Pointer.current.position.ReadValue();
        else
            return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = pointerPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            var selectable = r.gameObject.GetComponent<Selectable>();
            if (selectable != null && selectable.IsInteractable())
            {
                return true;
            }
        }

        return false;
    }
}

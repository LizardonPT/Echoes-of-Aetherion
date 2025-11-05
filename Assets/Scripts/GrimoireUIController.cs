using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrimoireUIController : MonoBehaviour
{
    [SerializeField] private Grimoire grimoireUI;

    [SerializeField] private InputActionReference toggleGrimoireAction;

    private bool isGrimoireOpen = false;

    public void Start()
    {
        isGrimoireOpen = false;
        grimoireUI.Hide();
    }
    
    public void Update()
    {
        if (toggleGrimoireAction.action.triggered)
        {
            if (!isGrimoireOpen)
            {
                grimoireUI.Show();
            }
            else
            {
                grimoireUI.Hide();
            }
            isGrimoireOpen = !isGrimoireOpen;
        }
    }
}

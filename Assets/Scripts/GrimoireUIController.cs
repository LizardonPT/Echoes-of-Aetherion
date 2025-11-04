using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimoireUIController : MonoBehaviour
{
    [SerializeField] private Grimoire grimoireUI;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (grimoireUI.isActiveAndEnabled == false)
            {
                grimoireUI.Show();
            }
            else
            {
                grimoireUI.Hide();
            }
        }
    }
}

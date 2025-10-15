using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pausedMenu;

    public void TogglePause()
    {
        if (pausedMenu.activeSelf)
        {
            pausedMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            pausedMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}

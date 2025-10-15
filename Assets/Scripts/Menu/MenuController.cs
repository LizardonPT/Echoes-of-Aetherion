using UnityEngine;
using UnityEngine.UI;

namespace EchoesOfAetherion.Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pausedMenu;

        public void ShowPauseMenu()
        {
            pausedMenu?.SetActive(true);
        }

        public void HidePauseMenu()
        {
            pausedMenu?.SetActive(false);
        }
    }
}

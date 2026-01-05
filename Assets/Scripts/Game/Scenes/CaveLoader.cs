using EchoesOfEtherion.Game.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveLoader : MonoBehaviour
{
    [SerializeField] private string sceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoader.Instance.LoadPrimaryScene(sceneName);
        }
    }
}

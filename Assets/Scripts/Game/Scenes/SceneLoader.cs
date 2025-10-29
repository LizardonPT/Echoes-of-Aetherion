using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using FMODUnity;
using EchoesOfEtherion.ScriptableObjects.Channels;

namespace EchoesOfEtherion.Game.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Channel Reference")]
        [SerializeField] private SceneLoaderChannel sceneLoaderChannel;

        [Header("UI References")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;

        [Header("Camera References")]
        [SerializeField] private Camera managersCamera;
        [SerializeField] private StudioListener studioListener;

        private Camera gameCamera;
        private readonly WaitForSeconds wfs = new WaitForSeconds(0.5f);

        private void OnEnable()
        {
            sceneLoaderChannel.OnLoadSceneAdditiveRequested += LoadSceneAdditive;
            sceneLoaderChannel.OnSwitchSceneRequested += SwitchToScene;
        }

        private void OnDisable()
        {
            sceneLoaderChannel.OnLoadSceneAdditiveRequested -= LoadSceneAdditive;
            sceneLoaderChannel.OnSwitchSceneRequested -= SwitchToScene;
        }

        private void Start()
        {
#if UNITY_EDITOR
            // If more than one scene is loaded (e.g. Managers + Gameplay Scene)
            if (SceneManager.sceneCount > 1)
            {
                Scene activeGameplayScene = FindNonManagerScene();

                if (activeGameplayScene.IsValid())
                {
                    SetupForExistingGameplayScene(activeGameplayScene);
                    sceneLoaderChannel.NotifySceneLoaded(activeGameplayScene.name);
                    return;
                }
            }
#endif
            // Normal boot path (only Managers loaded)
            sceneLoaderChannel.RequestLoadSceneAdditive("MainMenu");
        }

        /// <summary>
        /// Finds any loaded scene that isn't the Managers scene.
        /// </summary>
        private Scene FindNonManagerScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Managers" && scene.isLoaded)
                    return scene;
            }

            return default;
        }

        /// <summary>
        /// Sets up camera, listener, and state for direct scene play (Editor only).
        /// </summary>
        private void SetupForExistingGameplayScene(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Camera cam = root.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    gameCamera = cam;
                    break;
                }
            }

            if (gameCamera != null)
            {
                managersCamera.enabled = false;
                gameCamera.enabled = true;

                if (studioListener != null)
                    studioListener.AttenuationObject = gameCamera.gameObject;
            }
            else
            {
                Debug.LogWarning($"[SceneLoader] No camera found in scene {scene.name}!");
            }
        }

        private void LoadSceneAdditive(string sceneName)
        {
            StartCoroutine(LoadSceneAdditiveAsync(sceneName));
        }

        private IEnumerator LoadSceneAdditiveAsync(string sceneName)
        {
            loadingScreen?.SetActive(true);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                if (progressBar != null)
                    progressBar.value = op.progress;
                yield return null;
            }

            loadingScreen?.SetActive(false);
            sceneLoaderChannel.NotifySceneLoaded(sceneName);
        }

        private void SwitchToScene(string newScene, string oldScene)
        {
            StartCoroutine(SwitchSceneAsync(newScene, oldScene));
        }

        private IEnumerator SwitchSceneAsync(string newScene, string oldScene)
        {
            loadingScreen?.SetActive(true);
            managersCamera.enabled = true;

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = false;

            while (loadOp.progress < 0.9f)
            {
                if (progressBar != null)
                    progressBar.value = loadOp.progress;
                yield return null;
            }

            yield return wfs;

            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone)
                yield return null;

            Scene loadedScene = SceneManager.GetSceneByName(newScene);
            SetupForExistingGameplayScene(loadedScene);

            if (!string.IsNullOrEmpty(oldScene))
            {
                yield return SceneManager.UnloadSceneAsync(oldScene);
                sceneLoaderChannel.NotifySceneUnloaded(oldScene);
            }

            loadingScreen?.SetActive(false);
            sceneLoaderChannel.NotifySceneLoaded(newScene);
        }
    }
}

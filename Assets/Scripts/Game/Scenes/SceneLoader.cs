using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

        private Camera gameCamera;
        private readonly WaitForSeconds wfs = new(0.5f);

        private string currentSceneName;
        private readonly List<string> loadedAdditiveScenes = new();

        public string CurrentSceneName => currentSceneName;

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
            InitializeSceneSystem();
        }

        private void InitializeSceneSystem()
        {
#if UNITY_EDITOR
            // Handle editor play mode with existing scenes
            if (SceneManager.sceneCount > 1)
            {
                List<Scene> gameplayScenes = FindAllGameplayScenes();

                if (gameplayScenes.Count == 1)
                {
                    // Single gameplay scene found, use it as current
                    Scene gameplayScene = gameplayScenes[0];
                    currentSceneName = gameplayScene.name;
                    SetupForExistingGameplayScene(gameplayScene);
                    sceneLoaderChannel.NotifySceneLoaded(currentSceneName);
                    return;
                }
                else if (gameplayScenes.Count > 1)
                {
                    // Multiple scenes loaded - clean up and start fresh
                    Debug.LogWarning($"[SceneLoader] Multiple gameplay scenes found in editor. Cleaning up and loading MainMenu.");
                    StartCoroutine(CleanupAndLoadMainMenu());
                    return;
                }
            }
#endif
            // Normal boot path - only Managers scene is loaded
            LoadInitialScene();
        }

        private IEnumerator CleanupAndLoadMainMenu()
        {
            loadingScreen?.SetActive(true);

            // Unload all non-manager scenes
            List<AsyncOperation> unloadOperations = new List<AsyncOperation>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Managers" && scene.isLoaded)
                {
                    unloadOperations.Add(SceneManager.UnloadSceneAsync(scene));
                }
            }

            // Wait for all unloads to complete
            foreach (var op in unloadOperations)
            {
                while (!op.isDone)
                    yield return null;
            }

            loadingScreen?.SetActive(false);

            // Load main menu
            LoadInitialScene();
        }

        private void LoadInitialScene()
        {
            currentSceneName = "MainMenu";
            sceneLoaderChannel.RequestLoadSceneAdditive(currentSceneName);
        }

        /// <summary>
        /// Finds all loaded scenes that aren't the Managers scene.
        /// </summary>
        private List<Scene> FindAllGameplayScenes()
        {
            List<Scene> gameplayScenes = new();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Managers" && scene.isLoaded)
                    gameplayScenes.Add(scene);
            }
            return gameplayScenes;
        }

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
            // Ensure time scale is normal for loading
            Time.timeScale = 1f;

            loadingScreen?.SetActive(true);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                if (progressBar != null)
                    progressBar.value = op.progress;
                yield return null;
            }

            // Update scene tracking
            if (!loadedAdditiveScenes.Contains(sceneName))
            {
                loadedAdditiveScenes.Add(sceneName);
            }

            if (string.IsNullOrEmpty(currentSceneName))
            {
                currentSceneName = sceneName;
            }

            loadingScreen?.SetActive(false);
            sceneLoaderChannel.NotifySceneLoaded(sceneName);
        }

        private void SwitchToScene(string newScene)
        {
            StartCoroutine(SwitchSceneAsync(newScene, currentSceneName ?? ""));
        }

        private IEnumerator SwitchSceneAsync(string newScene, string oldScene)
        {
            // Ensure time scale is normal for loading
            Time.timeScale = 1f;

            loadingScreen?.SetActive(true);
            managersCamera.enabled = true;

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            loadOp.allowSceneActivation = false;

            float progress = 0f;
            while (progress < 0.9f)
            {
                progress = Mathf.MoveTowards(progress, loadOp.progress, Time.unscaledDeltaTime);
                if (progressBar != null)
                    progressBar.value = progress;
                yield return null;
            }

            yield return wfs;

            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone)
            {
                progress = Mathf.MoveTowards(progress, 1f, Time.unscaledDeltaTime);
                if (progressBar != null)
                    progressBar.value = progress;
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(newScene);
            SetupForExistingGameplayScene(loadedScene);

            // Update scene tracking
            currentSceneName = newScene;
            
            if (!loadedAdditiveScenes.Contains(newScene))
            {
                loadedAdditiveScenes.Add(newScene);
            }

            // Unload old scene if specified
            if (!string.IsNullOrEmpty(oldScene) && oldScene != "Managers")
            {
                yield return SceneManager.UnloadSceneAsync(oldScene);
                loadedAdditiveScenes.Remove(oldScene);
                sceneLoaderChannel.NotifySceneUnloaded(oldScene);
            }

            loadingScreen?.SetActive(false);
            sceneLoaderChannel.NotifySceneLoaded(newScene);
        }

        public string GetCurrentSceneName()
        {
            return currentSceneName;
        }

        public void RestartCurrentScene()
        {
            sceneLoaderChannel.RequestSwitchScene(currentSceneName);
        }
    }
}

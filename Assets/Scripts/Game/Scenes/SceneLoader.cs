using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EchoesOfEtherion.Game.Scenes
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [Header("UI References")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;

        [Header("Camera References")]
        [SerializeField] private Camera managersCamera;

        private Camera gameCamera;

        private string currentSceneName;
        private readonly List<string> loadedAdditiveScenes = new();

        public string CurrentSceneName => currentSceneName;

        public Action<string> SceneLoaded;
        public Action<string> SceneUnloaded;
        public Action<string> LoadingScene;
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
                    SceneLoaded?.Invoke(currentSceneName);
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
            LoadSceneAdditive(currentSceneName);
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

        public void LoadSceneAdditive(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("[SceneLoader] Invalid scene load request.");
                return;
            }
            LoadingScene?.Invoke(sceneName);
            StartCoroutine(LoadSceneAdditiveAsync(sceneName));
        }

        private IEnumerator LoadSceneAdditiveAsync(string sceneName)
        {
            loadingScreen?.SetActive(true);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = true;
            managersCamera.enabled = true;

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

            managersCamera.enabled = false;
            loadingScreen?.SetActive(false);
            SceneLoaded?.Invoke(sceneName);
        }

        public void SwitchToScene(string newScene)
        {
            if (string.IsNullOrEmpty(newScene) || string.IsNullOrWhiteSpace(newScene))
            {
                Debug.LogWarning("[SceneLoader] Invalid scene switch request.");
                return;
            }
            LoadingScene?.Invoke(newScene);
            StartCoroutine(SwitchSceneAsync(newScene, currentSceneName ?? ""));
        }

        private IEnumerator SwitchSceneAsync(string newScene, string oldScene)
        {
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
                SceneUnloaded?.Invoke(oldScene);
            }

            managersCamera.enabled = false;
            loadingScreen?.SetActive(false);
            SceneLoaded?.Invoke(newScene);
        }

        public string GetCurrentSceneName()
        {
            return currentSceneName;
        }

        public void RestartCurrentScene()
        {
            SwitchToScene(currentSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}

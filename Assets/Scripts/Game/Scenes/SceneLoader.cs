using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using EchoesOfEtherion.Game.Helpers;

namespace EchoesOfEtherion.Game.Scenes
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        [Header("UI References")]
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;

        private string currentPrimaryScene;
        private readonly List<string> loadedAuxiliaryScenes = new();

        public Action<string> SceneLoaded;
        public Action<string> SceneUnloaded;
        public Action<string> LoadingScene;

        public string CurrentPrimaryScene => new(currentPrimaryScene);

        private void Start()
        {
#if UNITY_EDITOR
            // Editor startup handling: detect gameplay scenes pre-loaded
            if (SceneManager.sceneCount > 1)
            {
                List<Scene> primaries = FindPrimaryScenesInEditor();

                if (primaries.Count == 1)
                {
                    // Start from existing primary scene
                    currentPrimaryScene = primaries[0].name;
                    SceneLoaded?.Invoke(currentPrimaryScene);
                    return;
                }
                else if (primaries.Count > 1)
                {
                    Debug.LogWarning("[SceneLoader] Multiple primary scenes detected on play. Resetting to MainMenu.");
                    StartCoroutine(CleanupAndLoad("MainMenu"));
                    return;
                }
            }
#endif
            // Normal boot: Only Managers is loaded
            LoadPrimaryScene("MainMenu");
        }

        public void LoadPrimaryScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("[SceneLoader] Invalid primary scene request.");
                return;
            }

            LoadingScene?.Invoke(sceneName);
            StartCoroutine(SwitchPrimaryAsync(sceneName));
        }

        private IEnumerator SwitchPrimaryAsync(string newScene)
        {
            loadingScreen?.SetActive(true);

            string oldScene = currentPrimaryScene;

            // Unload old primary scene
            if (!string.IsNullOrEmpty(oldScene) && oldScene != "Managers")
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldScene);
                while (!unloadOp.isDone) yield return null;

                SceneUnloaded?.Invoke(oldScene);
            }

            // Load new primary scene
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
            while (!loadOp.isDone)
            {
                if (progressBar != null)
                    progressBar.value = loadOp.progress;
                yield return null;
            }

            currentPrimaryScene = newScene;
            SceneLoaded?.Invoke(newScene);

            loadingScreen?.SetActive(false);
        }

        public void LoadAuxiliaryScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("[SceneLoader] Invalid auxiliary scene request.");
                return;
            }

            LoadingScene?.Invoke(sceneName);
            StartCoroutine(LoadAuxSceneAsync(sceneName));
        }

        private IEnumerator LoadAuxSceneAsync(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            while (!op.isDone)
            {
                if (progressBar != null)
                    progressBar.value = op.progress;
                yield return null;
            }

            if (!loadedAuxiliaryScenes.Contains(sceneName))
                loadedAuxiliaryScenes.Add(sceneName);

            SceneLoaded?.Invoke(sceneName);
        }

        public void UnloadAuxiliaryScene(string sceneName)
        {
            if (!loadedAuxiliaryScenes.Contains(sceneName))
                return;

            StartCoroutine(UnloadAuxSceneAsync(sceneName));
        }

        private IEnumerator UnloadAuxSceneAsync(string sceneName)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

            while (!op.isDone)
                yield return null;

            loadedAuxiliaryScenes.Remove(sceneName);
            SceneUnloaded?.Invoke(sceneName);
        }

#if UNITY_EDITOR
        private List<Scene> FindPrimaryScenesInEditor()
        {
            List<Scene> primaries = new();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);

                if (s.name != "Managers" && s.isLoaded)
                    primaries.Add(s);
            }

            return primaries;
        }

        private IEnumerator CleanupAndLoad(string primaryScene)
        {
            loadingScreen?.SetActive(true);

            // Unload everything except Managers
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.name != "Managers" && s.isLoaded)
                {
                    AsyncOperation op = SceneManager.UnloadSceneAsync(s);
                    while (!op.isDone) yield return null;
                }
            }

            loadingScreen?.SetActive(false);

            // Then load the clean initial scene
            LoadPrimaryScene(primaryScene);
        }
#endif

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

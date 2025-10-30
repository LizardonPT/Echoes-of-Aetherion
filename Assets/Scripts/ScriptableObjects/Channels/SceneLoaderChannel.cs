using System;
using UnityEngine;

namespace EchoesOfEtherion.ScriptableObjects.Channels
{
    [CreateAssetMenu(fileName = "SceneLoaderChannel", menuName = "Scriptable Objects/Channels/SceneLoaderChannel")]
    public class SceneLoaderChannel : ScriptableObject
    {
        public Action<string> OnLoadSceneAdditiveRequested;
        public Action<string> OnSwitchSceneRequested;

        public Action<string> OnSceneLoaded;
        public Action<string> OnSceneUnloaded;

        public Action<string, string> OnCurrentSceneChanged;

        public void RequestLoadSceneAdditive(string sceneName)
        {
            OnLoadSceneAdditiveRequested?.Invoke(sceneName);
        }

        public void RequestSwitchScene(string newScene)
        {
            OnSwitchSceneRequested?.Invoke(newScene);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        internal void NotifySceneLoaded(string sceneName)
        {
            OnSceneLoaded?.Invoke(sceneName);
        }

        internal void NotifySceneUnloaded(string sceneName)
        {
            OnSceneUnloaded?.Invoke(sceneName);
        }

        internal void NotifyCurrentSceneChanged(string newScene, string oldScene)
        {
            OnCurrentSceneChanged?.Invoke(newScene, oldScene);
        }
    }
}
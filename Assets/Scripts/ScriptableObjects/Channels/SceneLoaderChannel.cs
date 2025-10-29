using System;
using UnityEngine;

namespace EchoesOfEtherion.ScriptableObjects.Channels
{
    [CreateAssetMenu(fileName = "SceneLoaderChannel", menuName = "Scriptable Objects/Channels/SceneLoaderChannel")]
    public class SceneLoaderChannel : ScriptableObject
    {
        public Action<string> OnLoadSceneAdditiveRequested;
        public Action<string, string> OnSwitchSceneRequested;
        
        public Action<string> OnSceneLoaded;
        public Action<string> OnSceneUnloaded;

        public void RequestLoadSceneAdditive(string sceneName)
        {
            OnLoadSceneAdditiveRequested?.Invoke(sceneName);
        }

        public void RequestSwitchScene(string newScene, string oldScene)
        {
            OnSwitchSceneRequested?.Invoke(newScene, oldScene);
        }

        internal void NotifySceneLoaded(string sceneName)
        {
            OnSceneLoaded?.Invoke(sceneName);
        }

        internal void NotifySceneUnloaded(string sceneName)
        {
            OnSceneUnloaded?.Invoke(sceneName);
        }
    }
}

using EchoesOfEtherion.Game.States;
using EchoesOfEtherion.Menu;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.ScriptableObjects.Channels;
using EchoesOfEtherion.ScriptableObjects.Utils;
using EchoesOfEtherion.StateMachine;
using UnityEngine;

namespace EchoesOfEtherion.Game
{
    [RequireComponent(typeof(TickController))]
    [RequireComponent(typeof(GamePauser))]
    public class GameMaster : MonoBehaviour
    {
        [field: Header("ScriptableObjects")]
        [field: SerializeField] public InputReader InputReader { get; private set; }
        [field: SerializeField] public SceneLoaderChannel SceneLoaderChannel { get; private set; }

        [field: Space]
        [field: Header("References")]
        [field: SerializeField] public PauseMenu PauseMenu { get; private set; }

        public FiniteStateMachine<GameMaster> StateMachine { get; private set; }
        private TickController tickController;
        private GamePauser gamePauser;

        private bool gamePaused = false;
        private bool isLoading = false;

        private void Awake()
        {
            tickController ??= GetComponent<TickController>();
            gamePauser ??= GetComponent<GamePauser>();
            SetupStateMachine();
            SetupEventListeners();
        }

        private void OnDestroy()
        {
            RemoveEventListeners();
        }

        private void SetupEventListeners()
        {
            if (SceneLoaderChannel != null)
            {
                SceneLoaderChannel.OnLoadSceneAdditiveRequested += OnSceneLoadStarted;
                SceneLoaderChannel.OnSwitchSceneRequested += OnSceneLoadStarted;
                SceneLoaderChannel.OnSceneLoaded += OnSceneLoadCompleted;
            }
        }

        private void RemoveEventListeners()
        {
            if (SceneLoaderChannel != null)
            {
                SceneLoaderChannel.OnLoadSceneAdditiveRequested -= OnSceneLoadStarted;
                SceneLoaderChannel.OnSwitchSceneRequested -= OnSceneLoadStarted;
                SceneLoaderChannel.OnSceneLoaded -= OnSceneLoadCompleted;
            }
        }

        private void OnSceneLoadStarted(string sceneName)
        {
            OnSceneLoadStarted(sceneName, null);
        }

        private void OnSceneLoadStarted(string newScene, string oldScene)
        {
            if (!isLoading)
            {
                isLoading = true;
                StateMachine.ChangeState<GameLoadingState>();
            }
        }

        private void OnSceneLoadCompleted(string sceneName)
        {
            if (isLoading)
            {
                isLoading = false;
                // Return to gameplay state after loading completes
                StateMachine.ChangeState<GameplayState>();
            }
        }

        public void PauseGame()
        {
            if (!gamePaused && !isLoading)
            {
                tickController.SetPaused(true);
                gamePauser.PauseGame();
                Time.timeScale = 0f;
                gamePaused = true;
            }
        }

        public void ResumeGame()
        {
            if (gamePaused && !isLoading)
            {
                tickController.SetPaused(false);
                gamePauser.ResumeGame();
                Time.timeScale = 1f;
                gamePaused = false;
            }
        }

        public void SetPausedWithoutTimeScale(bool paused)
        {
            if (gamePaused != paused && !isLoading)
            {
                tickController.SetPaused(paused);
                if (paused)
                {
                    gamePauser.PauseGame();
                }
                else
                {
                    gamePauser.ResumeGame();
                }
                gamePaused = paused;
            }
        }

        private void Update()
        {
            StateMachine?.Update();
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<GameMaster>(this);

            StateMachine.AddState<GameplayState>(new GameplayState());
            StateMachine.AddState<GamePauseState>(new GamePauseState());
            StateMachine.AddState<GameLoadingState>(new GameLoadingState());

            StateMachine.ChangeState<GameplayState>();
        }
    }
}

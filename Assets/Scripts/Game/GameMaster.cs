using System;
using EchoesOfEtherion.Game.Scenes;
using EchoesOfEtherion.Game.States;
using EchoesOfEtherion.Menu;
using EchoesOfEtherion.Player.Components;
using EchoesOfEtherion.ScriptableObjects.Utils;
using EchoesOfEtherion.StateMachine;
using UnityEditor;
using UnityEngine;

namespace EchoesOfEtherion.Game
{
    [RequireComponent(typeof(TickController))]
    [RequireComponent(typeof(GamePauser))]
    public class GameMaster : Singleton<GameMaster>
    {
        [field: Header("ScriptableObjects")]
        [field: SerializeField] public InputReader InputReader { get; private set; }

        [field: Space]
        [field: Header("References")]
        [field: SerializeField] public PauseMenu PauseMenu { get; private set; }

        public FiniteStateMachine<GameMaster> StateMachine { get; private set; }
        private GamePauser gamePauser;

        public Action GamePaused;
        public Action GameResumed;

        private bool gamePaused = false;
        private bool isLoading = false;

        protected override void Awake()
        {
            base.Awake();

            gamePauser ??= GetComponent<GamePauser>();
            SetupStateMachine();
            SetupEventListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveEventListeners();
        }

        private void SetupEventListeners()
        {
            SceneLoader.Instance.LoadingScene += OnSceneLoadStarted;
            SceneLoader.Instance.SceneLoaded += OnSceneLoadCompleted;
            SceneLoader.Instance.SceneUnloaded += OnSceneLoadCompleted;
        }

        private void RemoveEventListeners()
        {
            SceneLoader.Instance.LoadingScene -= OnSceneLoadStarted;
            SceneLoader.Instance.SceneLoaded -= OnSceneLoadCompleted;
            SceneLoader.Instance.SceneUnloaded -= OnSceneLoadCompleted;
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
                StateMachine.ChangeState<GameplayState>();
            }
        }

        public void PauseGame()
        {
            if (!gamePaused && !isLoading)
            {
                gamePaused = true;
                TickController.Instance.SetPaused(true);
                gamePauser?.PauseGame();
                GamePaused?.Invoke();
                StateMachine.ChangeState<GamePauseState>();
            }
        }

        public void ResumeGame()
        {
            if (gamePaused && !isLoading)
            {
                gamePaused = false;
                TickController.Instance.SetPaused(false);
                gamePauser?.ResumeGame();
                GameResumed?.Invoke();
                StateMachine.ChangeState<GameplayState>();
            }
        }

        public void TogglePauseGame()
        {
            if (gamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void SetPausedWithoutTimeScale(bool paused)
        {
            TickController.Instance.SetPaused(paused);
            if (paused)
                gamePauser?.PauseGame();
            else
                gamePauser?.ResumeGame();
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

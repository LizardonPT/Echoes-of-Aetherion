using System;
using EchoesOfEtherion.Game.Scenes;
using EchoesOfEtherion.Game.States;
using EchoesOfEtherion.Menu;
using EchoesOfEtherion.ScriptableObjects.Utils;
using EchoesOfEtherion.StateMachine;
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

        [Header("Debug")]
        [SerializeField] private bool enableLogging = false;

        public FiniteStateMachine<GameMaster> StateMachine { get; private set; }
        private GamePauser _gamePauser;
        private TickController _tickController;

        public event Action GamePaused;
        public event Action GameResumed;
        public event Action GameplayStarted;
        public event Action LoadingStarted;

        public bool IsPaused => StateMachine.CurrentState is GamePauseState;
        public bool IsLoading => StateMachine.CurrentState is GameLoadingState;
        public bool IsInGameplay => StateMachine.CurrentState is GameplayState;
        public bool IsInMenu => StateMachine.CurrentState is GameMenuState;


        protected override void Awake()
        {
            base.Awake();

            _gamePauser = GetComponent<GamePauser>();
            _tickController = GetComponent<TickController>();

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
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadingScene += OnSceneLoadStarted;
                SceneLoader.Instance.SceneLoaded += OnSceneLoadCompleted;
            }
        }

        private void RemoveEventListeners()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadingScene -= OnSceneLoadStarted;
                SceneLoader.Instance.SceneLoaded -= OnSceneLoadCompleted;
            }
        }

        private void OnSceneLoadStarted(string sceneName)
        {
            StateMachine.ChangeState<GameLoadingState>();
        }

        private void OnSceneLoadCompleted(string sceneName)
        {
            // Determine which state to transition to based on the loaded scene
            if (sceneName == "MainMenu")
            {
                StateMachine.ChangeState<GameMenuState>();
            }
            else
            {
                StateMachine.ChangeState<GameplayState>();
            }
        }

        public void RequestPause()
        {
            if (IsInGameplay)
            {
                StateMachine.ChangeState<GamePauseState>();
            }
        }

        public void RequestResume()
        {
            if (IsPaused)
            {
                StateMachine.ChangeState<GameplayState>();
            }
        }

        public void TogglePauseGame()
        {
            if (!IsPaused && !IsInMenu)
                RequestPause();
            else if (!IsInMenu)
                RequestGameplay();
        }

        public void RequestMenu()
        {
            StateMachine.ChangeState<GameMenuState>();
        }

        public void RequestGameplay()
        {
            StateMachine.ChangeState<GameplayState>();
        }

        public void RequestLoading()
        {
            StateMachine.ChangeState<GameLoadingState>();
        }

        internal void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        internal void SetTickPaused(bool paused)
        {
            _tickController.SetPaused(paused);
        }

        internal void SetGamePaused(bool paused)
        {
            if (paused)
            {
                _gamePauser.PauseGame();
                GamePaused?.Invoke();
            }
            else
            {
                _gamePauser.ResumeGame();
                GameResumed?.Invoke();
            }
        }

        internal void ShowPauseMenu()
        {
            PauseMenu?.ShowPauseMenu();
        }

        internal void HidePauseMenu()
        {
            PauseMenu?.HidePauseMenu();
        }

        internal void InvokeGameplayStarted()
        {
            GameplayStarted?.Invoke();
        }

        internal void InvokeLoadingStarted()
        {
            LoadingStarted?.Invoke();
        }

        private void Update()
        {
            StateMachine?.Update();
        }

        private void FixedUpdate()
        {
            StateMachine?.FixedUpdate();
        }

        private void SetupStateMachine()
        {
            StateMachine = new FiniteStateMachine<GameMaster>(this);

            // Register all game states
            StateMachine.AddState<GameplayState>(new GameplayState());
            StateMachine.AddState<GamePauseState>(new GamePauseState());
            StateMachine.AddState<GameLoadingState>(new GameLoadingState());
            StateMachine.AddState<GameMenuState>(new GameMenuState());

            // Start in loading state
            StateMachine.ChangeState<GameLoadingState>();
        }

        internal void Log(string message)
        {
            if (enableLogging)
                Debug.Log($"[GameMaster] {message}");
        }
    }
}
using System;
using UnityEngine;
using Sjur.Resources;
using Sjur.Buildings;
using Sjur.Themes;
using Sjur.Combat;
using Sjur.Lanes;

namespace Sjur.Core
{
    /// <summary>
    /// Main game manager coordinating all game systems
    /// Handles match flow, win conditions, and system initialization
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private ResourceManager team0Resources;
        [SerializeField] private ResourceManager team1Resources;
        [SerializeField] private ThemeManager themeManager;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private LaneManager laneManager;

        [Header("Spawners")]
        [SerializeField] private UnitSpawner team0Spawner;
        [SerializeField] private UnitSpawner team1Spawner;

        [Header("Bases")]
        [SerializeField] private Building team0Base;
        [SerializeField] private Building team1Base;

        [Header("Center Objective")]
        [SerializeField] private CenterObjective centerObjective;

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Initializing;
        [SerializeField] private int winningTeam = -1;

        public GameState CurrentState => currentState;
        public int WinningTeam => winningTeam;

        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnGameEnded;

        private void Awake()
        {
            InitializeGame();
        }

        private void Start()
        {
            SetupEventListeners();
            StartGame();
        }

        private void InitializeGame()
        {
            Debug.Log("Initializing game systems...");

            // Validate all required components
            if (team0Resources == null || team1Resources == null)
            {
                Debug.LogError("Resource managers not assigned!");
            }

            if (team0Spawner == null || team1Spawner == null)
            {
                Debug.LogError("Unit spawners not assigned!");
            }

            if (team0Base == null || team1Base == null)
            {
                Debug.LogError("Base buildings not assigned!");
            }

            SetGameState(GameState.Initializing);
        }

        private void SetupEventListeners()
        {
            // Base destruction events
            if (team0Base != null)
            {
                team0Base.OnBuildingDestroyed += OnTeam0BaseDestroyed;
            }

            if (team1Base != null)
            {
                team1Base.OnBuildingDestroyed += OnTeam1BaseDestroyed;
            }

            // Center objective events
            if (centerObjective != null)
            {
                centerObjective.OnObjectiveCaptured += OnCenterObjectiveCaptured;
            }
        }

        private void StartGame()
        {
            Debug.Log("Starting game...");
            SetGameState(GameState.Playing);
        }

        private void OnTeam0BaseDestroyed(Building destroyedBase)
        {
            // Team 1 wins
            EndGame(1);
        }

        private void OnTeam1BaseDestroyed(Building destroyedBase)
        {
            // Team 0 wins
            EndGame(0);
        }

        private void OnCenterObjectiveCaptured(int teamId)
        {
            Debug.Log($"Center objective captured by Team {teamId}");
            // Bonus already applied by CenterObjective component
        }

        private void EndGame(int victoryTeam)
        {
            if (currentState == GameState.GameOver)
                return;

            winningTeam = victoryTeam;
            SetGameState(GameState.GameOver);

            Debug.Log($"Game Over! Team {victoryTeam} wins!");
            OnGameEnded?.Invoke(victoryTeam);
        }

        private void SetGameState(GameState newState)
        {
            if (currentState == newState)
                return;

            currentState = newState;
            OnGameStateChanged?.Invoke(newState);

            Debug.Log($"Game state changed to: {newState}");
        }

        /// <summary>
        /// Get resource manager for specific team
        /// </summary>
        public ResourceManager GetResourceManager(int teamId)
        {
            return teamId == 0 ? team0Resources : team1Resources;
        }

        /// <summary>
        /// Get unit spawner for specific team
        /// </summary>
        public UnitSpawner GetUnitSpawner(int teamId)
        {
            return teamId == 0 ? team0Spawner : team1Spawner;
        }

        /// <summary>
        /// Get base building for specific team
        /// </summary>
        public Building GetBase(int teamId)
        {
            return teamId == 0 ? team0Base : team1Base;
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("Restarting game...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                Time.timeScale = 0f;
                SetGameState(GameState.Paused);
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetGameState(GameState.Playing);
            }
        }

        private void OnDestroy()
        {
            // Clean up event listeners
            if (team0Base != null)
            {
                team0Base.OnBuildingDestroyed -= OnTeam0BaseDestroyed;
            }

            if (team1Base != null)
            {
                team1Base.OnBuildingDestroyed -= OnTeam1BaseDestroyed;
            }

            if (centerObjective != null)
            {
                centerObjective.OnObjectiveCaptured -= OnCenterObjectiveCaptured;
            }
        }
    }

    /// <summary>
    /// Game state enum
    /// </summary>
    public enum GameState
    {
        Initializing,
        Playing,
        Paused,
        GameOver
    }
}

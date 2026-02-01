using UnityEngine;
using UnityEngine.SceneManagement;

namespace MaskMYDrama.Core
{
    public enum GameState
    {
        MainMenu,
        InLevel,
        Paused,
        Victory,
        GameOver
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        public int currentLevel = 1;
        public int maxLevel = 3;
        
        private GameState currentState = GameState.MainMenu;
        
        public GameState CurrentState => currentState;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void StartNewGame()
        {
            currentLevel = 1;
            currentState = GameState.InLevel;
            LoadLevel(currentLevel);
        }
        
        public void LoadGame(int level)
        {
            currentLevel = level;
            currentState = GameState.InLevel;
            LoadLevel(level);
        }
        
        private void LoadLevel(int level)
        {
            // Load the combat scene
            SceneManager.LoadScene("CombatScene");
        }
        
        public void PauseGame()
        {
            if (currentState == GameState.InLevel)
            {
                currentState = GameState.Paused;
                Time.timeScale = 0f;
            }
        }
        
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                currentState = GameState.InLevel;
                Time.timeScale = 1f;
            }
        }
        
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            LoadLevel(currentLevel);
        }
        
        public void BackToMenu()
        {
            Time.timeScale = 1f;
            currentState = GameState.MainMenu;
            SceneManager.LoadScene("StartMenu");
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
        
        public void OnLevelComplete()
        {
            currentLevel++;
            if (currentLevel > maxLevel)
            {
                // All levels completed
                currentState = GameState.Victory;
            }
            else
            {
                // Load next level
                LoadLevel(currentLevel);
            }
        }
        
        public void OnPlayerDefeated()
        {
            currentState = GameState.GameOver;
            // Show game over UI
        }
    }
}


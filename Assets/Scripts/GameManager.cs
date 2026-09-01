using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int score = 0;
    public int level = 1;
    public bool isPaused = false;
    public string playerName = "";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configurar pantalla completa
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame(string name)
    {
        playerName = name;
        score = 0;
        level = 1;
        isPaused = false;
        SceneManager.LoadScene("SampleScene");
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void AddScore(int points)
    {
        score += points;
        // Lógica para subir de nivel
        int previousLevel = level;
        level = score / 100;

        // Si subió de nivel, notificar al personaje
        if (level > previousLevel)
        {
            CharacterReactions characterReactions = FindObjectOfType<CharacterReactions>();
            if (characterReactions != null)
            {
                characterReactions.OnLevelUp(level);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }
}

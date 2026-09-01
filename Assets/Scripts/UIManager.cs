using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject pausePanel; // Nuevo panel de pausa
    public GameObject gameOverPanel; // Panel de Game Over
    public TMP_Text finalScoreText; // Puntaje final en Game Over
    public TMP_InputField nameInput;
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text playerNameText;
    public TMP_Text warningText; // Texto de advertencia
    public Slider volumeSlider; // Barra de volumen música
    public Slider sfxVolumeSlider; // Barra de volumen efectos
    public LoadingScreen loadingScreen; // Pantalla de carga
    public CountdownScreen countdownScreen; // Cuenta regresiva
    public MenuStatsDisplay menuStatsDisplay; // Estadísticas del menú

    void Start()
    {
        // Configurar slider de volumen
        if (volumeSlider != null)
        {
            if (AudioManager.Instance != null)
                volumeSlider.value = AudioManager.Instance.musicVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Configurar slider de efectos
        if (sfxVolumeSlider != null)
        {
            if (AudioManager.Instance != null)
                sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        // Solo mostrar el menú si no hay jugador activo
        if (string.IsNullOrEmpty(GameManager.Instance.playerName))
        {
            ShowMenu();
        }
        else
        {
            ShowGame();
            UpdateScore(GameManager.Instance.score);
            UpdateLevel(GameManager.Instance.level);
        }
    }

    void Update()
    {
        bool canTogglePause = gamePanel != null && gamePanel.activeSelf
            && (menuPanel == null || !menuPanel.activeSelf)
            && (gameOverPanel == null || !gameOverPanel.activeSelf)
            && (loadingScreen == null || !loadingScreen.gameObject.activeSelf)
            && (countdownScreen == null || !countdownScreen.gameObject.activeSelf);

        if (!canTogglePause)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPauseButton();
        }
    }


    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Actualizar estadísticas del menú
        if (menuStatsDisplay != null)
        {
            menuStatsDisplay.UpdateStats();
        }

        // Limpiar el tablero y detener todo
        Board board = FindObjectOfType<Board>();
        if (board != null) board.ClearBoard();
        GameManager.Instance.ResumeGame();

        // Reanudar música del menú
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeMenuMusic();
    }

    public void OnVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSfxVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxVolume(value);
    }

    // MÉTODOS PARA BOTONES
    public void OnPlayButton()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();

        string playerName = nameInput.text.Trim();
        if (string.IsNullOrEmpty(playerName))
        {
            if (warningText != null)
            {
                warningText.text = "¡Por favor ingresa tu usuario!";
                warningText.gameObject.SetActive(true);
            }
            return;
        }
        if (warningText != null) warningText.gameObject.SetActive(false);

        string savedName = playerName;
        menuPanel.SetActive(false);

        // Mostrar pantalla de carga
        if (loadingScreen != null)
        {
            loadingScreen.StartLoading(() =>
            {
                GameManager.Instance.playerName = savedName;
                GameManager.Instance.score = 0;
                GameManager.Instance.level = 1;
                ShowGame();
                UpdateScore(0);
                UpdateLevel(1);

                // Pausar el juego hasta que termine la cuenta regresiva
                GameManager.Instance.PauseGame();

                if (countdownScreen != null)
                {
                    countdownScreen.StartCountdown(() =>
                    {
                        GameManager.Instance.ResumeGame();
                        // Generar la primera pieza
                        Spawner spawner = FindObjectOfType<Spawner>();
                        if (spawner != null) spawner.SpawnTetromino();
                        
                        // Pausar música del menú
                        if (AudioManager.Instance != null)
                            AudioManager.Instance.PauseMenuMusic();
                        
                        // Iniciar música de gameplay
                        if (GameplayMusicManager.Instance != null)
                        {
                            GameplayMusicManager.Instance.PlayRandomTrack();
                        }
                    });
                }
                else
                {
                    GameManager.Instance.ResumeGame();
                    Spawner spawner = FindObjectOfType<Spawner>();
                    if (spawner != null) spawner.SpawnTetromino();
                }
            });
        }
        else
        {
            GameManager.Instance.StartGame(savedName);
            ShowGame();
            UpdateScore(0);
            UpdateLevel(1);
        }
    }

    public void OnPauseButton()
    {
        if (GameManager.Instance.isPaused)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayResumeSound();
            GameManager.Instance.ResumeGame();
            if (pausePanel != null) pausePanel.SetActive(false);
            
            // Reanudar música de gameplay donde estaba
            if (GameplayMusicManager.Instance != null)
                GameplayMusicManager.Instance.ResumeMusic();
        }
        else
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayPauseSound();
            GameManager.Instance.PauseGame();
            if (pausePanel != null) pausePanel.SetActive(true);
            
            // Pausar música de gameplay
            if (GameplayMusicManager.Instance != null)
                GameplayMusicManager.Instance.PauseMusic();
        }
    }

    public void OnExitButton()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayButtonSound();
        GameManager.Instance.ExitGame();
    }

    public void ShowGame()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (playerNameText != null) playerNameText.text = GameManager.Instance.playerName;
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    public void UpdateLevel(int level)
    {
        if (levelText != null) levelText.text = level.ToString();
    }

    // GAME OVER
    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = GameManager.Instance.score.ToString();

        // Sonido de Game Over
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOverSound();

        // Parar música de gameplay
        if (GameplayMusicManager.Instance != null)
            GameplayMusicManager.Instance.StopMusic();

        GameManager.Instance.PauseGame();
    }

    public void OnRestartButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSfx();
            AudioManager.Instance.PlayButtonSound();
        }

        // Parar música de gameplay actual
        if (GameplayMusicManager.Instance != null)
            GameplayMusicManager.Instance.StopMusic();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        GameManager.Instance.ResumeGame();

        // Limpiar tablero
        Board board = FindObjectOfType<Board>();
        if (board != null) board.ClearBoard();

        string savedName = GameManager.Instance.playerName;

        if (loadingScreen != null)
        {
            loadingScreen.StartLoading(() =>
            {
                GameManager.Instance.score = 0;
                GameManager.Instance.level = 1;
                ShowGame();
                UpdateScore(0);
                UpdateLevel(1);

                GameManager.Instance.PauseGame();

                if (countdownScreen != null)
                {
                    countdownScreen.StartCountdown(() =>
                    {
                        GameManager.Instance.ResumeGame();
                        Spawner spawner = FindObjectOfType<Spawner>();
                        if (spawner != null) spawner.SpawnTetromino();
                        
                        // Pausar música del menú
                        if (AudioManager.Instance != null)
                            AudioManager.Instance.PauseMenuMusic();
                        
                        // Iniciar música de gameplay
                        if (GameplayMusicManager.Instance != null)
                        {
                            GameplayMusicManager.Instance.PlayRandomTrack();
                        }
                    });
                }
                else
                {
                    GameManager.Instance.ResumeGame();
                    Spawner spawner = FindObjectOfType<Spawner>();
                    if (spawner != null) spawner.SpawnTetromino();
                }
            });
        }
        else
        {
            GameManager.Instance.StartGame(savedName);
            ShowGame();
            UpdateScore(0);
            UpdateLevel(1);
        }
    }

    public void OnMenuButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSfx();
            AudioManager.Instance.PlayButtonSound();
        }

        // Parar música de gameplay
        if (GameplayMusicManager.Instance != null)
            GameplayMusicManager.Instance.StopMusic();

        // Reanudar música del menú
        if (AudioManager.Instance != null)
            AudioManager.Instance.ResumeMenuMusic();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        GameManager.Instance.playerName = "";
        GameManager.Instance.ResumeGame();

        // Limpiar tablero
        Board board = FindObjectOfType<Board>();
        if (board != null) board.ClearBoard();

        if (loadingScreen != null)
        {
            loadingScreen.StartLoading(() =>
            {
                ShowMenu();
            });
        }
        else
        {
            ShowMenu();
        }
    }

    // Ir al menú desde el panel de PAUSA
    public void OnPauseMenuButton()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSfx();
            AudioManager.Instance.PlayButtonSound();
        }
        if (pausePanel != null) pausePanel.SetActive(false);
        GameManager.Instance.ResumeGame();

        // Limpiar tablero
        Board board = FindObjectOfType<Board>();
        if (board != null) board.ClearBoard();

        GameManager.Instance.playerName = "";

        if (loadingScreen != null)
        {
            loadingScreen.StartLoading(() =>
            {
                ShowMenu();
            });
        }
        else
        {
            ShowMenu();
        }
    }

    // Verificar si el juego está activo (no en menú ni pausa ni game over)
    public bool IsGameActive()
    {
        if (gamePanel == null) return false;
        
        bool menuActive = menuPanel != null && menuPanel.activeSelf;
        bool pauseActive = pausePanel != null && pausePanel.activeSelf;
        bool gameOverActive = gameOverPanel != null && gameOverPanel.activeSelf;
        bool loadingActive = loadingScreen != null && loadingScreen.gameObject.activeSelf;
        bool countdownActive = countdownScreen != null && countdownScreen.gameObject.activeSelf;
        
        return gamePanel.activeSelf && !menuActive && !pauseActive && !gameOverActive && !loadingActive && !countdownActive;
    }
}

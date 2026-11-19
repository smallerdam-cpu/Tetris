using UnityEngine;
using System.Collections.Generic;

public class GameplayMusicManager : MonoBehaviour
{
    public static GameplayMusicManager Instance;
    
    [Header("Canciones de Gameplay")]
    public SongInfo[] gameplayTracks;

    private AudioSource musicSource;
    private int lastTrackIndex = -1;
    private SongInfoDisplay songInfoDisplay;
    private float pausedTime = 0f;
    private UIManager uiManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Crear AudioSource para la música de gameplay
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = false;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        songInfoDisplay = FindObjectOfType<SongInfoDisplay>();
        uiManager = FindObjectOfType<UIManager>(); // Cachemos la variable por rendimiento
    }

    void Update()
    {
        // 1. Sincronizar el volumen en tiempo real con el menú de ajustes
        if (AudioManager.Instance != null && musicSource != null)
        {
            musicSource.volume = AudioManager.Instance.musicVolume;
        }

        // Detectamos todos los estados de la UI
        bool isGamePanel = uiManager != null && uiManager.gamePanel != null && uiManager.gamePanel.activeSelf;
        bool isGameOver = uiManager != null && uiManager.gameOverPanel != null && uiManager.gameOverPanel.activeSelf;
        bool isCountdown = uiManager != null && uiManager.countdownScreen != null && uiManager.countdownScreen.gameObject.activeSelf;
        bool isLoading = uiManager != null && uiManager.loadingScreen != null && uiManager.loadingScreen.gameObject.activeSelf;

        // Solo puede sonar música si estamos en el panel de juego, y no hay paneles de interrupción activos
        bool canPlayMusic = isGamePanel && !isGameOver && !isCountdown && !isLoading;

        // 3. Hotkey para cambiar canción (ahora funciona en pausa)
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (canPlayMusic)
            {
                PlayRandomTrack();
                return;
            }
        }

        // 4. Reproducción automática al terminar
        if (musicSource.isPlaying == false && gameplayTracks.Length > 0)
        {
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.playerName))
            {
                if (canPlayMusic)
                {
                    PlayRandomTrack();
                }
            }
        }
    }
    

    public void PlayRandomTrack()
    {
        if (gameplayTracks.Length == 0)
        {
            Debug.LogWarning("No hay canciones asignadas en GameplayMusicManager");
            return;
        }

        // Seleccionar una canción aleatoria diferente a la anterior
        int randomIndex = lastTrackIndex;
        while (randomIndex == lastTrackIndex && gameplayTracks.Length > 1)
        {
            randomIndex = Random.Range(0, gameplayTracks.Length);
        }

        lastTrackIndex = randomIndex;
        SongInfo selectedSong = gameplayTracks[randomIndex];

        Debug.Log("Reproduciendo: " + selectedSong.songName + " - " + selectedSong.artistName);
        musicSource.clip = selectedSong.audioClip;
        musicSource.time = 0f; // Comenzar desde el inicio
        musicSource.Play();

        // Mostrar información de la canción
        if (songInfoDisplay != null)
        {
            songInfoDisplay.ShowSongInfo(selectedSong);
        }
    }

    public void PauseMusic()
    {
        pausedTime = musicSource.time;
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource.clip != null)
        {
            musicSource.time = pausedTime;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        pausedTime = 0f;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
}

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
    }

    void Update()
    {
        // Si la música terminó, reproducir otra aleatoria
        if (musicSource.isPlaying == false && gameplayTracks.Length > 0)
        {
            // Pero solo si hay música cargada y está dentro del juego
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.playerName))
            {
                // Verificar que no estamos en el menú ni en pausa
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null && uiManager.IsGameActive())
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

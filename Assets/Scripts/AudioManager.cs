using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Música de Fondo")]
    public AudioClip backgroundMusic;

    [Header("Líneas")]
    public AudioClip line1Sound;        // 1 Línea
    public AudioClip line2Sound;        // 2 Líneas
    public AudioClip line3Sound;        // 3 Líneas
    public AudioClip line4Sound;        // TETRIS! (4 Líneas)
    public AudioClip perfectClearSound; // Perfect Clear

    [Header("Piezas")]
    public AudioClip moveSound;         // Mover Pieza
    public AudioClip rotateSound;       // Rotar Pieza
    public AudioClip landSound;         // Bloquear/Aterrizar Pieza

    [Header("Juego")]
    public AudioClip comboSound;        // Combo
    public AudioClip levelUpSound;      // Subir de nivel
    public AudioClip gameOverSound;     // Game Over
    public AudioClip startGameSound;    // Inicio Partida
    public AudioClip countdownSound;    // Cuenta Atrás

    [Header("Menú")]
    public AudioClip buttonSound;       // Botón
    public AudioClip pauseSound;        // Pausa
    public AudioClip resumeSound;       // Reanudar

    [Header("Volumen")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (backgroundMusic != null && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMenuMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMenuMusic()
    {
        if (!musicSource.isPlaying && musicSource.clip != null)
            musicSource.UnPause();
    }

    public void StopAllSfx()
    {
        sfxSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    // --- LÍNEAS ---
    public void PlayLineClearSound(int lines)
    {
        AudioClip clip = null;
        switch (lines)
        {
            case 1: clip = line1Sound; break;
            case 2: clip = line2Sound; break;
            case 3: clip = line3Sound; break;
            case 4: clip = line4Sound; break;
        }
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayPerfectClearSound()
    {
        if (perfectClearSound != null)
            sfxSource.PlayOneShot(perfectClearSound, sfxVolume);
    }

    // --- PIEZAS ---
    public void PlayMoveSound()
    {
        if (moveSound != null)
            sfxSource.PlayOneShot(moveSound, sfxVolume);
    }

    public void PlayRotateSound()
    {
        if (rotateSound != null)
            sfxSource.PlayOneShot(rotateSound, sfxVolume);
    }

    public void PlayLandSound()
    {
        if (landSound != null)
            sfxSource.PlayOneShot(landSound, sfxVolume);
    }

    // --- JUEGO ---
    public void PlayComboSound()
    {
        if (comboSound != null)
            sfxSource.PlayOneShot(comboSound, sfxVolume);
    }

    public void PlayLevelUpSound()
    {
        if (levelUpSound != null)
            sfxSource.PlayOneShot(levelUpSound, sfxVolume);
    }

    public void PlayGameOverSound()
    {
        if (gameOverSound != null)
            sfxSource.PlayOneShot(gameOverSound, sfxVolume);
    }

    public void PlayStartGameSound()
    {
        if (startGameSound != null)
            sfxSource.PlayOneShot(startGameSound, sfxVolume);
    }

    public void PlayCountdownSound()
    {
        if (countdownSound != null)
            sfxSource.PlayOneShot(countdownSound, sfxVolume);
    }

    // --- MENÚ ---
    public void PlayButtonSound()
    {
        if (buttonSound != null)
            sfxSource.PlayOneShot(buttonSound, sfxVolume);
    }

    public void PlayPauseSound()
    {
        if (pauseSound != null)
            sfxSource.PlayOneShot(pauseSound, sfxVolume);
    }

    public void PlayResumeSound()
    {
        if (resumeSound != null)
            sfxSource.PlayOneShot(resumeSound, sfxVolume);
    }
}

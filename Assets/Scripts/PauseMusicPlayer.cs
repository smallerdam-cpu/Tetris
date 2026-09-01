using UnityEngine;

public class PauseMusicPlayer : MonoBehaviour
{
    [Header("Música de Pausa")]
    public AudioClip pauseMusic;
    
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        // Cuando el panel de pausa se activa
        if (pauseMusic != null && audioSource != null)
        {
            audioSource.clip = pauseMusic;
            audioSource.volume = 0.5f; // Volumen más bajo para la pausa
            audioSource.Play();
        }
    }

    void OnDisable()
    {
        // Cuando el panel de pausa se desactiva
        if (audioSource != null)
            audioSource.Stop();
    }
}

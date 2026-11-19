using UnityEngine;
using System.Collections;

public class IntroSequence : MonoBehaviour
{
    [Header("Configuración Visual")]
    public CanvasGroup[] logos; 
    public float fadeDuration = 2f; // Transiciones más suaves y lentas
    public float displayDuration = 4f; // Tiempo en pantalla por logo

    [Header("Configuración de Audio")]
    public AudioSource introAudio; 

    private UIManager uiManager;
    private bool isSkipped = false;

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        
        foreach (CanvasGroup logo in logos)
        {
            logo.alpha = 0f;
            logo.gameObject.SetActive(false);
        }

        StartCoroutine(PlayIntroSequence());
    }

    void Update()
    {
        // Saltar intro visual con cualquier tecla
        if (!isSkipped && Input.anyKeyDown)
        {
            isSkipped = true;
            StopAllCoroutines();
            FinishIntro(true); // Pasamos 'true' porque se omitió forzadamente
        }
    }

    IEnumerator PlayIntroSequence()
    {
        if (introAudio != null) introAudio.Play();

        // Secuencia de los 3 logos (aprox 20-25 segundos en total)
        foreach (CanvasGroup logo in logos)
        {
            logo.gameObject.SetActive(true);

            yield return Fade(logo, 0f, 1f);
            yield return new WaitForSeconds(displayDuration);
            yield return Fade(logo, 1f, 0f);
            
            logo.gameObject.SetActive(false);
            
            // Pequeño respiro en negro entre logos
            yield return new WaitForSeconds(0.5f);
        }

        // Finaliza la parte visual, pero dejamos que el audio termine naturalmente en el menú
        FinishIntro(false); 
    }

    IEnumerator Fade(CanvasGroup cg, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            yield return null;
        }
        cg.alpha = end;
    }

    void FinishIntro(bool skipped)
    {
        // Si el jugador omitió la intro, cortamos el audio. Si no, dejamos que termine de sonar en el menú.
        if (skipped && introAudio != null) 
        {
            introAudio.Stop();
        }
        
        gameObject.SetActive(false); 
        
        if (uiManager != null)
        {
            uiManager.ShowMenu();
        }
    }
}
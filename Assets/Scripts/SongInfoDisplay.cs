using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class SongInfoDisplay : MonoBehaviour
{
    public Image songImageIcon;
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI artistNameText;
    public CanvasGroup panelCanvasGroup;
    public float displayDuration = 10f;
    public float fadeDuration = 0.5f;

    private Coroutine displayCoroutine;

    void Start()
    {
        // Asegurarse de que el panel está invisible al principio
        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;
    }

    public void ShowSongInfo(SongInfo songInfo)
    {
        if (songInfo == null) return;

        // Si ya hay una corrutina, cancelarla
        if (displayCoroutine != null)
            StopCoroutine(displayCoroutine);

        // Actualizar la información
        if (songImageIcon != null && songInfo.songImage != null)
            songImageIcon.sprite = songInfo.songImage;

        if (songNameText != null)
            songNameText.text = songInfo.songName;

        if (artistNameText != null)
            artistNameText.text = songInfo.artistName;

        // Iniciar la corrutina de visualización
        displayCoroutine = StartCoroutine(DisplayRoutine());
    }

    private IEnumerator DisplayRoutine()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 1f;

        // Esperar 10 segundos
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;
    }
}

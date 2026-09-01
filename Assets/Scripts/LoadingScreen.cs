using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingBar;
    public TMP_Text loadingText;
    public float loadDuration = 2f; // Duración de la carga en segundos

    private System.Action onLoadComplete;

    public void StartLoading(System.Action callback)
    {
        gameObject.SetActive(true);
        onLoadComplete = callback;
        StartCoroutine(LoadProgress());
    }

    IEnumerator LoadProgress()
    {
        float elapsed = 0f;

        while (elapsed < loadDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / loadDuration);

            if (loadingBar != null)
                loadingBar.value = progress;

            if (loadingText != null)
                loadingText.text = "Cargando... " + Mathf.RoundToInt(progress * 100) + "%";

            yield return null;
        }

        if (loadingBar != null) loadingBar.value = 1f;
        if (loadingText != null) loadingText.text = "Cargando... 100%";

        yield return new WaitForSecondsRealtime(0.3f);

        gameObject.SetActive(false);

        if (onLoadComplete != null)
            onLoadComplete.Invoke();
    }
}

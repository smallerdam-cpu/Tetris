using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownScreen : MonoBehaviour
{
    public GameObject countdownPanel;
    public Image readyImage;    // Imagen "Preparate"
    public Image count3Image;   // Imagen "3"
    public Image count2Image;   // Imagen "2"
    public Image count1Image;   // Imagen "1"
    public Image goImage;       // Imagen "GO"

    public float displayTime = 0.8f; // Tiempo que se muestra cada imagen

    private System.Action onCountdownComplete;

    public void StartCountdown(System.Action callback)
    {
        onCountdownComplete = callback;
        countdownPanel.SetActive(true);
        HideAll();
        StartCoroutine(CountdownSequence());
    }

    void HideAll()
    {
        if (readyImage != null) readyImage.gameObject.SetActive(false);
        if (count3Image != null) count3Image.gameObject.SetActive(false);
        if (count2Image != null) count2Image.gameObject.SetActive(false);
        if (count1Image != null) count1Image.gameObject.SetActive(false);
        if (goImage != null) goImage.gameObject.SetActive(false);
    }

    IEnumerator CountdownSequence()
    {
        // Preparate
        if (readyImage != null)
        {
            readyImage.gameObject.SetActive(true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownSound();
            yield return new WaitForSecondsRealtime(displayTime * 1.5f);
            readyImage.gameObject.SetActive(false);
        }

        // 3
        if (count3Image != null)
        {
            count3Image.gameObject.SetActive(true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownSound();
            yield return new WaitForSecondsRealtime(displayTime);
            count3Image.gameObject.SetActive(false);
        }

        // 2
        if (count2Image != null)
        {
            count2Image.gameObject.SetActive(true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownSound();
            yield return new WaitForSecondsRealtime(displayTime);
            count2Image.gameObject.SetActive(false);
        }

        // 1
        if (count1Image != null)
        {
            count1Image.gameObject.SetActive(true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayCountdownSound();
            yield return new WaitForSecondsRealtime(displayTime);
            count1Image.gameObject.SetActive(false);
        }

        // GO!
        if (goImage != null)
        {
            goImage.gameObject.SetActive(true);
            if (AudioManager.Instance != null) AudioManager.Instance.PlayStartGameSound();
            yield return new WaitForSecondsRealtime(displayTime);
            goImage.gameObject.SetActive(false);
        }

        countdownPanel.SetActive(false);

        if (onCountdownComplete != null)
            onCountdownComplete.Invoke();
    }
}

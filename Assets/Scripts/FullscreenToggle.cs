using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    public Toggle fullscreenToggle;

    void Start()
    {
        // Obtener el estado actual de fullscreen
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
        }
    }

    private void OnFullscreenToggle(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}

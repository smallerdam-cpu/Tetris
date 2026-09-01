using UnityEngine;
using UnityEngine.UI;

public class ScrollViewMouseScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 10f;

    void Update()
    {
        if (scrollRect == null) return;

        // Detectar rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            // Cambiar posición vertical del scroll
            scrollRect.verticalNormalizedPosition += scroll * scrollSpeed * Time.deltaTime;
            
            // Clampar entre 0 y 1
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }
    }
}

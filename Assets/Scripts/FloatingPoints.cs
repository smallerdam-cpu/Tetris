using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FloatingPoints : MonoBehaviour
{
    private TextMeshProUGUI pointsText;
    private Image comboImage;
    private RectTransform rectTransform;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float duration = 1f;
    private float elapsedTime = 0f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        pointsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Interpolación suave hacia arriba
            rectTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // Desvanecimiento (Alpha)
            float currentAlpha = Mathf.Lerp(1f, 0f, progress);

            if (pointsText != null)
            {
                Color textColor = pointsText.color;
                textColor.a = currentAlpha;
                pointsText.color = textColor;
            }

            if (comboImage != null)
            {
                Color imgColor = comboImage.color;
                imgColor.a = currentAlpha;
                comboImage.color = imgColor;
            }

            if (elapsedTime >= duration)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Vector3 spawnPos, Vector3 targetPos, float animDuration, Image assignedImage)
    {
        startPosition = spawnPos;
        targetPosition = targetPos;
        duration = animDuration;
        comboImage = assignedImage;
        rectTransform.position = spawnPos;
    }
}
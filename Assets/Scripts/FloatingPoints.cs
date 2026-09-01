using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingPoints : MonoBehaviour
{
    private TextMeshProUGUI pointsText;
    private RectTransform rectTransform;
    private int points;
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

            // Interpolación suave hacia los puntos
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, progress);
            rectTransform.position = newPosition;

            // Desvanecimiento
            Color color = pointsText.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            pointsText.color = color;

            // Si es final, destruir
            if (elapsedTime >= duration)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Vector3 spawnPos, Vector3 targetPos, int pointValue, string bonusText, float animDuration = 1f, Color pointColor = default, Color bonusColor = default)
    {
        startPosition = spawnPos;
        targetPosition = targetPos;
        points = pointValue;
        duration = animDuration;
        rectTransform.position = spawnPos;

        // Usar colores por defecto si no se especifican
        if (pointColor == default) pointColor = Color.yellow;
        if (bonusColor == default) bonusColor = Color.green;

        // Mostrar texto con bonus y colores personalizados
        string pointsHex = ColorUtility.ToHtmlStringRGB(pointColor);
        string bonusHex = ColorUtility.ToHtmlStringRGB(bonusColor);
        
        pointsText.text = $"<color=#{pointsHex}>+{points}</color>\n<color=#{bonusHex}>{bonusText}</color>";
        pointsText.fontSize = 36;
        pointsText.alignment = TextAlignmentOptions.Center;
    }
}

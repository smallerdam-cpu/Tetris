using UnityEngine;
using TMPro;

public class MenuStatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentLevelText;

    void OnEnable()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (playerNameText != null)
        {
            if (string.IsNullOrEmpty(GameManager.Instance.playerName))
                playerNameText.text = "Usuario: -";
            else
                playerNameText.text = "Usuario: " + GameManager.Instance.playerName;
        }

        if (currentScoreText != null)
        {
            if (GameManager.Instance.score == 0)
                currentScoreText.text = "Score: Sin score";
            else
                currentScoreText.text = "Score: " + GameManager.Instance.score.ToString();
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = "Nivel: " + GameManager.Instance.level.ToString();
        }
    }
}

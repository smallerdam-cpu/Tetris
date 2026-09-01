using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreditsPanel : MonoBehaviour
{
    public GameObject creditsPanel;
    public TextMeshProUGUI creditsText;
    public Button closeButton;
    public Button openButton;

    [TextArea(10, 15)]
    public string creditsContent = @"
TETRIS GAME
═══════════════════════════════

DESARROLLO
• Desarrollador Principal: Tu Nombre
• Programación: C# - Unity

DISEÑO
• Diseño de Interfaz: Tu Nombre
• Diseño de Personajes: Tu Nombre

ARTE
• Sprites de Piezas: Tu Nombre
• Personajes Espectadores: Tu Nombre
• Temas Visuales: Tu Nombre

MÚSICA
• Composición Música Menú: Tu Nombre
• Composición Música Gameplay: Tu Nombre
• Canciones Gameplay: Lista de Artistas

SONIDO
• Efectos de Sonido: Tu Nombre
• Diseño de Audio: Tu Nombre

AGRADECIMIENTOS
Gracias a todos por jugar

═══════════════════════════════
© 2026 - Todos los derechos reservados
";

    void Start()
    {
        // Asegurarse de que el panel esté cerrado al inicio
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // Configurar el botón de abrir
        if (openButton != null)
            openButton.onClick.AddListener(OpenCredits);

        // Configurar el botón de cerrar
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCredits);

        // Mostrar el contenido de créditos
        if (creditsText != null)
            creditsText.text = creditsContent;
    }

    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}

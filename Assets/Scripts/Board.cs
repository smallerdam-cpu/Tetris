using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 20;
    public Transform[,] grid;
    private int totalLinesCleared = 0;

    [Header("Textos de Bonus")]
    public string bonusText1 = "BONUS!";
    public string bonusText2 = "DOUBLE BONUS!";
    public string bonusText3 = "TRIPLE BONUS!";
    public string bonusText4 = "MEGA BONUS!";

    [Header("Posición de Puntos Flotantes")]
    public Vector2 floatingPointsDirection = new Vector2(200, 400);
    public float floatingPointsDuration = 1f;

    [Header("Colores de Puntos Flotantes")]
    public Color pointsTextColor = Color.yellow;
    public Color bonusTextColor = Color.green;

    void Awake()
    {
        grid = new Transform[width, height];
    }

    public bool IsInsideBoard(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < width && (int)pos.y >= 0 && (int)pos.y < height);
    }

    public void AddToGrid(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            Vector2 pos = Round(block.position);
            if ((int)pos.y < height)
                grid[(int)pos.x, (int)pos.y] = block;
        }
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public void ClearLines()
    {
        int linesCleared = 0;

        for (int y = height - 1; y >= 0; y--)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                MoveAllLinesDown(y);
                y++; // Revisar la misma fila otra vez
                linesCleared++;
            }
        }

        if (linesCleared > 0)
        {
            // Sonido según cantidad de líneas
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLineClearSound(linesCleared);

            // Verificar Perfect Clear (tablero vacío)
            if (IsBoardEmpty())
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayPerfectClearSound();
            }

            // Puntaje: 1 línea=100, 2=300, 3=500, 4=800
            int[] scoreValues = { 0, 100, 300, 500, 800 };
            int points = scoreValues[Mathf.Min(linesCleared, 4)];

            // Texto de bonus según líneas
            string bonusText = "";
            switch (linesCleared)
            {
                case 1:
                    bonusText = bonusText1;
                    break;
                case 2:
                    bonusText = bonusText2;
                    break;
                case 3:
                    bonusText = bonusText3;
                    break;
                case 4:
                    bonusText = bonusText4;
                    break;
                default:
                    bonusText = "INCREDIBLE!";
                    break;
            }

            // Mostrar puntos flotantes
            ShowFloatingPoints(points, bonusText);

            int previousLevel = GameManager.Instance.level;
            GameManager.Instance.AddScore(points);

            // Sonido al subir de nivel
            if (GameManager.Instance.level > previousLevel)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayLevelUpSound();
            }

            // Cambiar tema cada 4 líneas destruidas
            totalLinesCleared += linesCleared;
            if (totalLinesCleared % 4 == 0 && ThemeManager.Instance != null)
            {
                ThemeManager.Instance.ChangeTheme();
            }

            // Actualizar la UI
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.UpdateScore(GameManager.Instance.score);
                uiManager.UpdateLevel(GameManager.Instance.level);
            }

            // Reacción del personaje espectador
            CharacterReactions characterReactions = FindObjectOfType<CharacterReactions>();
            if (characterReactions != null)
            {
                characterReactions.OnLinesCleared(linesCleared);
            }
        }
    }

    bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
                return false;
        }
        return true;
    }

    void DeleteLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    void MoveAllLinesDown(int startY)
    {
        for (int y = startY + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }

    public bool IsGameOver()
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, height - 1] != null)
                return true;
        }
        return false;
    }

    bool IsBoardEmpty()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (grid[x, y] != null)
                    return false;
        return true;
    }

    public void ClearBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }

        // Destruir todos los tetrominos activos
        Tetromino[] tetrominos = FindObjectsOfType<Tetromino>();
        foreach (Tetromino t in tetrominos)
        {
            Destroy(t.gameObject);
        }
    }

    public void SetTetrominoSprites(Sprite[] sprites)
    {
        // Cambiar sprites de todos los bloques en el grid
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    SpriteRenderer spriteRenderer = grid[x, y].GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && sprites.Length > 0)
                    {
                        // Asignar un sprite aleatorio del tema
                        int spriteIndex = Random.Range(0, sprites.Length);
                        spriteRenderer.sprite = sprites[spriteIndex];
                    }
                }
            }
        }
    }

    void ShowFloatingPoints(int points, string bonusText)
    {
        // Crear un objeto temporal para mostrar los puntos
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Crear un GameObject con TextMeshProUGUI
        GameObject floatingObj = new GameObject("FloatingPoints");
        floatingObj.transform.SetParent(canvas.transform, false);

        // Agregar TextMeshProUGUI
        TextMeshProUGUI tmpText = floatingObj.AddComponent<TextMeshProUGUI>();
        RectTransform rectTransform = floatingObj.GetComponent<RectTransform>();

        // Posición inicial: centro del tablero
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.sizeDelta = new Vector2(400, 200);

        // Agregar script FloatingPoints
        FloatingPoints floatingPoints = floatingObj.AddComponent<FloatingPoints>();

        // Posición destino: usar la configuración personalizada
        Vector3 targetPos = new Vector3(floatingPointsDirection.x, floatingPointsDirection.y, 0);

        // Inicializar la animación con colores personalizados
        floatingPoints.Initialize(rectTransform.position, targetPos, points, bonusText, floatingPointsDuration, pointsTextColor, bonusTextColor);
    }
}

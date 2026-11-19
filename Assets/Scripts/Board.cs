using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 20;
    public Transform[,] grid;
    private int totalLinesCleared = 0;

    [Header("Textos de Bonus")]
    public Sprite combo1Image; // Imagen para la linea 1 
    public Sprite combo2Image; // imagen para la linea 2
    public Sprite combo3Image; // imagen para la linea 3
    public Sprite combo4Image; // imagen para COMBO linea 4

    [Header("Posición de Puntos Flotantes")]
    public Vector2 floatingPointsDirection = new Vector2(200, 400);
    public float floatingPointsDuration = 1f;
    public Color pointsTextColor = Color.yellow;

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

            // NUEVO: Asignar imagen según líneas
            Sprite comboSprite = null;
            switch (linesCleared)
            {
                case 1: comboSprite = combo1Image; break;
                case 2: comboSprite = combo2Image; break;
                case 3: comboSprite = combo3Image; break;
                case 4: comboSprite = combo4Image; break;
            }

            // Mostrar puntos flotantes y la imagen
            ShowFloatingPoints(points, comboSprite);

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
        // 1. Limpiamos los bloques individuales que están guardados en la matriz
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

        // 2. Aspiradora total: Buscamos todos los objetos sueltos en la escena
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // Si el objeto es un clon de una pieza o un fantasma, lo destruimos sin piedad
            if ((obj.name.Contains("Tetromino") && obj.name.Contains("(Clone)")) || obj.name.Contains("GhostPiece"))
            {
                Destroy(obj);
            }
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

    void ShowFloatingPoints(int points, Sprite comboSprite)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Crear contenedor principal
        GameObject floatingObj = new GameObject("FloatingPoints");
        floatingObj.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = floatingObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector3.zero;

        // Configurar texto numérico
        TextMeshProUGUI tmpText = floatingObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = $"+{points}";
        tmpText.fontSize = 50;
        tmpText.fontStyle = FontStyles.Bold;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = pointsTextColor;

        // Configurar Imagen de Combo
        Image imgComp = null;
        if (comboSprite != null)
        {
            GameObject imgObj = new GameObject("ComboImage");
            imgObj.transform.SetParent(floatingObj.transform, false);
            imgComp = imgObj.AddComponent<Image>();
            imgComp.sprite = comboSprite;
            imgComp.SetNativeSize(); // Ajusta automáticamente al tamaño real de tu sprite
            
            // Posicionar la imagen ligeramente arriba del texto
            imgObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 80);
        }

        FloatingPoints floatingPoints = floatingObj.AddComponent<FloatingPoints>();
        Vector3 targetPos = new Vector3(floatingPointsDirection.x, floatingPointsDirection.y, 0);
        
        floatingPoints.Initialize(rectTransform.position, targetPos, floatingPointsDuration, imgComp);
    }
}

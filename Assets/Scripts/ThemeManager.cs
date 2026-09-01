using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance;

    [System.Serializable]
    public class Theme
    {
        public string themeName;
        public Sprite[] blockSprites; // Sprites para los 7 tetrominós: I, O, T, S, Z, J, L
    }

    public Theme[] themes;
    private int currentThemeIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Cargar tema guardado
        currentThemeIndex = PlayerPrefs.GetInt("CurrentTheme", 0);
    }

    void Start()
    {
        // Aplicar tema inicial
        ApplyTheme(currentThemeIndex);
    }

    public void ChangeTheme()
    {
        currentThemeIndex++;
        if (currentThemeIndex >= themes.Length)
            currentThemeIndex = 0;

        ApplyTheme(currentThemeIndex);
        PlayerPrefs.SetInt("CurrentTheme", currentThemeIndex);
        PlayerPrefs.Save();

        Debug.Log($"🎨 Tema cambiado a: {themes[currentThemeIndex].themeName}");
    }

    public void ApplyTheme(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= themes.Length) return;

        currentThemeIndex = themeIndex;
        Theme currentTheme = themes[themeIndex];

        // Cambiar sprites de todos los bloques del tablero
        Board board = FindObjectOfType<Board>();
        if (board != null)
        {
            board.SetTetrominoSprites(currentTheme.blockSprites);
        }

        // Cambiar sprites de los tetrominós activos
        Tetromino[] activeTetrominös = FindObjectsOfType<Tetromino>();
        foreach (Tetromino tetromino in activeTetrominös)
        {
            tetromino.UpdateSprite(currentTheme.blockSprites);
        }
    }

    public Sprite[] GetCurrentBlockSprites()
    {
        if (currentThemeIndex >= 0 && currentThemeIndex < themes.Length)
            return themes[currentThemeIndex].blockSprites;
        return themes[0].blockSprites;
    }

    public Theme GetCurrentTheme()
    {
        return themes[currentThemeIndex];
    }
}

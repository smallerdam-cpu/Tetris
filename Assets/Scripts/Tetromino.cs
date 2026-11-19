using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public float fallTime = 1f;
    private float fallTimeOriginal;
    private float fallTimer; 
    private Board board;
    private Spawner spawner;
    private UIManager uiManager;

    [Header("Input")]
    public float inputHoldDelay = 0.18f;
    public float inputRepeatRate = 0.08f;
    private int heldHorizontalDirection = 0;
    private float nextHorizontalRepeatTime = 0f;


    [HideInInspector] public int pieceIndex;
    public GameObject ghostPiece; // (Asegúrate de quitarle el 'private' para que el Spawner pueda destruirlo)

    void Start()
    {
        board = FindObjectOfType<Board>();
        spawner = FindObjectOfType<Spawner>();
        uiManager = FindObjectOfType<UIManager>();
        
        fallTimeOriginal = fallTime;
        fallTimer = fallTime;

        // --- NUEVO: Crear el fantasma al nacer la pieza ---
        CreateGhostPiece();
    }

    void Update()
    {
        // Añadimos la protección "|| board == null" para evitar que piezas huérfanas causen errores rojos.
        if (GameManager.Instance.isPaused || board == null) return;

        fallTimer -= Time.deltaTime;

        if (fallTimer <= 0)
        {
            transform.position += Vector3.down;

            // Nota: Ahora IsValidPosition pide un Transform. Le pasamos 'transform' (esta pieza).
            if (!IsValidPosition(transform))
            {
                LockPiece();
                return;
            }
            fallTimer = fallTime; 
        }

        int horizontalInput = GetHorizontalStepInput();
        bool moved = false; // Bandera para saber si nos movimos y actualizar el fantasma

        if (Input.GetKeyDown(KeyCode.LeftArrow) || horizontalInput < 0)
        {
            Move(Vector3.left);
            moved = true;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayMoveSound();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || horizontalInput > 0)
        {
            Move(Vector3.right);
            moved = true;
            if (AudioManager.Instance != null) AudioManager.Instance.PlayMoveSound();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 1. Giramos la pieza primero
            transform.Rotate(0, 0, -90);
            
            // 2. Revisamos si choca con la pared o con otras piezas
            if (!IsValidPosition(transform))
            {
                // 3. Si choca, intentamos empujarla (Wall Kick)
                if (TryWallKick())
                {
                    moved = true;
                    if (AudioManager.Instance != null) AudioManager.Instance.PlayRotateSound();
                }
                else
                {
                    // 4. Si ningún empujón funciona, cancelamos la rotación
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                // La rotación fue válida sin necesidad de empujar
                moved = true;
                if (AudioManager.Instance != null) AudioManager.Instance.PlayRotateSound();
            }
        }

            

        // --- NUEVO: Si la pieza se movió o rotó, actualizamos la posición del fantasma ---
        if (moved)
        {
            UpdateGhostPiece();
        }

        bool softDropHeld = Input.GetKey(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") < -0.5f;
        fallTime = softDropHeld ? 0.05f : fallTimeOriginal;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        // --- NUEVO: Input para Sistema Hold ---
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (spawner.canHold)
            {
                spawner.HoldPiece(this);
                return; // Salimos de la función porque esta pieza acaba de ser destruida
            }
        }
    }

    // --- NUEVAS FUNCIONES DE GHOST PIECE ---

    /// <summary>
    /// Clona la pieza actual, le quita la lógica y la hace semi-transparente.
    /// </summary>
    void CreateGhostPiece()
    {
        ghostPiece = Instantiate(gameObject, transform.position, transform.rotation);
        
        // ¡LA SOLUCIÓN! Apagamos el script inmediatamente antes de destruirlo.
        // Así evitamos que el fantasma ejecute su propio Start() y cree más clones.
        Tetromino ghostScript = ghostPiece.GetComponent<Tetromino>();
        if (ghostScript != null)
        {
            ghostScript.enabled = false;
            Destroy(ghostScript);
        }

        // Le cambiamos el nombre para distinguirlo en tu jerarquía
        ghostPiece.name = "GhostPiece_" + gameObject.name;

        SpriteRenderer[] renderers = ghostPiece.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
        {
            Color color = sr.color;
            color.a = 0.3f; // 30% de opacidad
            sr.color = color;
        }

        UpdateGhostPiece();
    }

    /// <summary>
    /// Calcula dónde caería la pieza actual y mueve el fantasma ahí.
    /// </summary>
    void UpdateGhostPiece()
    {
        if (ghostPiece == null) return;

        // 1. El fantasma copia la posición y rotación actual de la pieza real
        ghostPiece.transform.position = transform.position;
        ghostPiece.transform.rotation = transform.rotation;

        // 2. Simulamos su caída hacia abajo hasta que toque algo
        while (IsValidPosition(ghostPiece.transform))
        {
            ghostPiece.transform.position += Vector3.down;
        }

        // 3. Como el bucle paró al chocar, lo subimos una posición (la posición válida final)
        ghostPiece.transform.position += Vector3.up;
    }

    /// <summary>
    /// Centraliza la lógica de bloquear la pieza cuando toca el fondo.
    /// </summary>
    void LockPiece()
    {
        transform.position += Vector3.up;
        board.AddToGrid(transform);
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayLandSound();
        
        // --- NUEVO: Destruir el fantasma cuando la pieza real se bloquea ---
        if (ghostPiece != null) Destroy(ghostPiece);

        board.ClearLines();

        if (board.IsGameOver())
        {
            if (uiManager != null) uiManager.ShowGameOver();
            enabled = false;
            return;
        }

        spawner.SpawnTetromino();
        enabled = false;
    }

    /// <summary>
    /// Intenta desplazar la pieza si al rotar queda dentro de una pared u otro bloque.
    /// </summary>
    bool TryWallKick()
    {
        // Lista de posiciones a probar: derecha, izquierda, derecha x2, izquierda x2, y arriba
        Vector3[] wallKickOffsets = new Vector3[]
        {
            new Vector3(1, 0, 0),   // 1 casilla a la Derecha
            new Vector3(-1, 0, 0),  // 1 casilla a la Izquierda
            new Vector3(2, 0, 0),   // 2 casillas a la Derecha (útil para la pieza I larga)
            new Vector3(-2, 0, 0),  // 2 casillas a la Izquierda (útil para la pieza I larga)
            new Vector3(0, 1, 0)    // 1 casilla Arriba (por si choca con el suelo)
        };

        foreach (Vector3 offset in wallKickOffsets)
        {
            // Probamos mover la pieza
            transform.position += offset;
            
            if (IsValidPosition(transform))
            {
                return true; // Encontramos un espacio libre, dejamos la pieza aquí
            }
            
            // Si sigue chocando, la regresamos a donde estaba y probamos el siguiente offset
            transform.position -= offset;
        }

        return false; // No hubo espacio para rotar de ninguna forma
    }

    // ----------------------------------------

    int GetHorizontalStepInput()
    {
        float axis = Input.GetAxisRaw("Horizontal");
        int direction = 0;
        
        if (axis > 0.5f) direction = 1;
        else if (axis < -0.5f) direction = -1;

        if (direction == 0)
        {
            heldHorizontalDirection = 0;
            return 0;
        }
        if (heldHorizontalDirection != direction)
        {
            heldHorizontalDirection = direction;
            nextHorizontalRepeatTime = Time.time + inputHoldDelay;
            return direction;
        }
        if (Time.time >= nextHorizontalRepeatTime)
        {
            nextHorizontalRepeatTime = Time.time + inputRepeatRate;
            return direction;
        }
        return 0;
    }

    void HardDrop()
    {
        while (IsValidPosition(transform))
        {
            transform.position += Vector3.down;
        }
        LockPiece(); // Usamos la nueva función centralizada
    }

    void Move(Vector3 direction)
    {
        transform.position += direction;
        if (!IsValidPosition(transform))
            transform.position -= direction;
    }

    /// <summary>
    /// Verifica si la posición del transform enviado es válida dentro del tablero.
    /// </summary>
    bool IsValidPosition(Transform testTransform)
    {
        // Si el tablero no existe en este milisegundo, cancelamos la validación.
        if (board == null) return false; 

        foreach (Transform block in testTransform)
        {
            Vector2 pos = board.Round(block.position);
            
            if ((int)pos.x < 0 || (int)pos.x >= board.width || (int)pos.y < 0)
                return false;
                
            if ((int)pos.y >= board.height)
                continue;
                
            if (board.grid[(int)pos.x, (int)pos.y] != null)
                return false;
        }
        return true;
    }

    public void UpdateSprite(Sprite[] sprites)
    {
        foreach (Transform block in transform)
        {
            SpriteRenderer spriteRenderer = block.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && sprites.Length > 0)
            {
                int spriteIndex = Random.Range(0, sprites.Length);
                spriteRenderer.sprite = sprites[spriteIndex];
            }
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Asignar en orden: I, O, T, S, Z, J, L")]
    public GameObject[] tetrominos; 

    private Queue<int> nextPiecesQueue = new Queue<int>();

    // --- NUEVO: Variables para el sistema Hold ---
    public int heldPieceIndex = -1; // -1 significa que la reserva está vacía
    public bool canHold = true;     // Bandera para evitar abusos del botón Hold

    void Start()
    {
        nextPiecesQueue.Enqueue(Random.Range(0, tetrominos.Length));
        nextPiecesQueue.Enqueue(Random.Range(0, tetrominos.Length));
    }

    public void SpawnTetromino()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;

        int nextPieceIndex = nextPiecesQueue.Dequeue();
        SpawnPieceByIndex(nextPieceIndex); // Usamos la nueva función para instanciar
        
        nextPiecesQueue.Enqueue(Random.Range(0, tetrominos.Length));
        
        NextPiecesDisplay display = FindObjectOfType<NextPiecesDisplay>();
        if (display != null) display.UpdateDisplay(GetNextPieces());

        // Al generar una pieza normal (tocó el suelo), permitimos usar Hold otra vez
        canHold = true; 
    }

    /// <summary>
    /// Instancia una pieza específica usando su índice.
    /// </summary>
    private void SpawnPieceByIndex(int index)
    {
        GameObject newPiece = Instantiate(tetrominos[index], transform.position, Quaternion.identity);
        
        // Le pasamos su "identidad" a la pieza
        Tetromino tetro = newPiece.GetComponent<Tetromino>();
        if (tetro != null) 
        {
            tetro.pieceIndex = index;
        }
    }

    /// <summary>
    /// Guarda la pieza actual y saca la de reserva.
    /// </summary>
    public void HoldPiece(Tetromino currentPiece)
    {
        if (!canHold) return; // Si ya usamos Hold este turno, ignoramos el comando

        int currentOriginalIndex = currentPiece.pieceIndex;
        
        // Destruimos la pieza que estaba cayendo
        Destroy(currentPiece.gameObject);
        if (currentPiece.ghostPiece != null) Destroy(currentPiece.ghostPiece); // Destruir su fantasma

        if (heldPieceIndex == -1)
        {
            // Primera vez que guardamos: la reserva estaba vacía
            heldPieceIndex = currentOriginalIndex;
            SpawnTetromino(); // Sacamos una nueva de la cola normal
        }
        else
        {
            // Ya había una pieza guardada: hacemos el intercambio
            int temp = heldPieceIndex;
            heldPieceIndex = currentOriginalIndex;
            SpawnPieceByIndex(temp); // Instanciamos la que estaba guardada
        }

        canHold = false; // Bloqueamos el Hold hasta la siguiente caída

        // En vez de buscar el script viejo, usamos el unificado
        NextPiecesDisplay display = FindObjectOfType<NextPiecesDisplay>();
        if (display != null) display.UpdateHoldDisplay(heldPieceIndex);
    }

    /// <summary>
    /// Resetea la memoria del generador para un juego nuevo.
    /// </summary>
    public void ResetSpawner()
    {
        heldPieceIndex = -1; // Borra la pieza guardada
        canHold = true;
        nextPiecesQueue.Clear(); // Limpia las próximas piezas
        
        // Genera 2 nuevas para el nuevo juego
        nextPiecesQueue.Enqueue(Random.Range(0, tetrominos.Length));
        nextPiecesQueue.Enqueue(Random.Range(0, tetrominos.Length));
        
        // Actualiza la UI para vaciar los recuadros
        NextPiecesDisplay display = FindObjectOfType<NextPiecesDisplay>();
        if (display != null)
        {
            display.UpdateDisplay(GetNextPieces());
            if (display.holdPieceImage != null) display.holdPieceImage.enabled = false;
        }
    }

    public int[] GetNextPieces()
    {
        return nextPiecesQueue.ToArray();
    }
}
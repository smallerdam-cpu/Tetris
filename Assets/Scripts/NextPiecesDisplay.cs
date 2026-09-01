using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NextPiecesDisplay : MonoBehaviour
{
    [Header("Próximas Piezas")]
    public Image nextPiece1Image;
    public Image nextPiece2Image;
    public TextMeshProUGUI nextPiecesLabel;

    [Header("Pieza Guardada (Hold)")]
    public Image holdPieceImage; // NUEVO: Aquí asignaremos la imagen del cuadro verde

    [Header("7 Imágenes de Piezas")]
    public Sprite spriteI;
    public Sprite spriteO;
    public Sprite spriteT;
    public Sprite spriteS;
    public Sprite spriteZ;
    public Sprite spriteJ;
    public Sprite spriteL;
    
    private Sprite[] allSprites;
    private Spawner spawner;

    void Start()
    {
        spawner = FindObjectOfType<Spawner>();
        allSprites = new Sprite[] { spriteI, spriteO, spriteT, spriteS, spriteZ, spriteJ, spriteL };
        
        // Ocultar la imagen de reserva al iniciar
        if (holdPieceImage != null)
            holdPieceImage.enabled = false;
            
        if (spawner != null)
        {
            UpdateDisplay(spawner.GetNextPieces());
        }
    }

    public void UpdateDisplay(int[] nextPieces)
    {
        if (nextPieces.Length >= 2)
        {
            if (nextPieces[0] >= 0 && nextPieces[0] < allSprites.Length && allSprites[nextPieces[0]] != null)
            {
                if (nextPiece1Image != null)
                {
                    nextPiece1Image.sprite = allSprites[nextPieces[0]];
                    nextPiece1Image.enabled = true;
                }
            }
            if (nextPieces[1] >= 0 && nextPieces[1] < allSprites.Length && allSprites[nextPieces[1]] != null)
            {
                if (nextPiece2Image != null)
                {
                    nextPiece2Image.sprite = allSprites[nextPieces[1]];
                    nextPiece2Image.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// NUEVA FUNCIÓN: Muestra la pieza que el jugador acaba de guardar.
    /// </summary>
    public void UpdateHoldDisplay(int pieceIndex)
    {
        if (pieceIndex >= 0 && pieceIndex < allSprites.Length && holdPieceImage != null)
        {
            holdPieceImage.sprite = allSprites[pieceIndex];
            holdPieceImage.enabled = true;
        }
    }
}
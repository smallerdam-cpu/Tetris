using UnityEngine;
using UnityEngine.UI;

public class HoldPieceDisplay : MonoBehaviour
{
    [Header("UI Image para la pieza guardada")]
    public Image holdPieceImage;

    [Header("7 Imágenes de Piezas (Orden: I, O, T, S, Z, J, L)")]
    public Sprite spriteI;
    public Sprite spriteO;
    public Sprite spriteT;
    public Sprite spriteS;
    public Sprite spriteZ;
    public Sprite spriteJ;
    public Sprite spriteL;
    
    private Sprite[] allSprites;

    void Start()
    {
        // Agrupamos los sprites en el mismo orden que tu Spawner
        allSprites = new Sprite[] { spriteI, spriteO, spriteT, spriteS, spriteZ, spriteJ, spriteL };
        
        if (holdPieceImage != null)
            holdPieceImage.enabled = false; // Oculto al inicio porque no hay nada guardado
    }

    /// <summary>
    /// Actualiza la imagen en pantalla con la pieza guardada.
    /// </summary>
    public void UpdateDisplay(int pieceIndex)
    {
        if (pieceIndex >= 0 && pieceIndex < allSprites.Length && holdPieceImage != null)
        {
            holdPieceImage.sprite = allSprites[pieceIndex];
            holdPieceImage.enabled = true;
        }
    }
}
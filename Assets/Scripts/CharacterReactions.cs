using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterReactions : MonoBehaviour
{
    public Image leftCharacter;
    public Image rightCharacter;

    [Header("Reacciones por líneas")]
    public Sprite reactionDefault;
    public Sprite reaction1Line;
    public Sprite reaction2Lines;
    public Sprite reaction3Lines;
    public Sprite reaction4Lines; // Tetris

    [Header("Reacciones por niveles")]
    public Sprite reactionLevel10;
    public Sprite reactionLevel20;
    public Sprite reactionLevel30;
    public Sprite reactionLevel50;
    public Sprite reactionLevel100;

    [Header("Animación")]
    public float floatingSpeed = 2f;
    public float floatingAmount = 10f;
    public float fadeDuration = 0.3f;

    private float reactionDuration = 1.5f;
    private float reactionTimer = 0f;
    private Sprite lastReaction;
    private Vector2 leftOriginalPos;
    private Vector2 rightOriginalPos;
    private CanvasGroup leftCanvasGroup;
    private CanvasGroup rightCanvasGroup;

    void Start()
    {
        // Guardar posiciones originales
        if (leftCharacter != null)
            leftOriginalPos = leftCharacter.GetComponent<RectTransform>().anchoredPosition;
        if (rightCharacter != null)
            rightOriginalPos = rightCharacter.GetComponent<RectTransform>().anchoredPosition;

        // Agregar CanvasGroup si no existen
        if (leftCharacter != null && leftCharacter.GetComponent<CanvasGroup>() == null)
            leftCharacter.gameObject.AddComponent<CanvasGroup>();
        if (rightCharacter != null && rightCharacter.GetComponent<CanvasGroup>() == null)
            rightCharacter.gameObject.AddComponent<CanvasGroup>();

        leftCanvasGroup = leftCharacter != null ? leftCharacter.GetComponent<CanvasGroup>() : null;
        rightCanvasGroup = rightCharacter != null ? rightCharacter.GetComponent<CanvasGroup>() : null;

        // Mostrar reacción por defecto
        ShowReaction(reactionDefault);
    }

    void Update()
    {
        // Animación de flotación
        if (leftCharacter != null)
        {
            RectTransform leftRect = leftCharacter.GetComponent<RectTransform>();
            float floatY = Mathf.Sin(Time.time * floatingSpeed) * floatingAmount;
            leftRect.anchoredPosition = new Vector2(leftOriginalPos.x, leftOriginalPos.y + floatY);
        }

        if (rightCharacter != null)
        {
            RectTransform rightRect = rightCharacter.GetComponent<RectTransform>();
            float floatY = Mathf.Sin(Time.time * floatingSpeed) * floatingAmount;
            rightRect.anchoredPosition = new Vector2(rightOriginalPos.x, rightOriginalPos.y + floatY);
        }

        // Temporizador para volver a reacción neutra
        if (reactionTimer > 0)
        {
            reactionTimer -= Time.deltaTime;
            if (reactionTimer <= 0)
            {
                ShowReaction(reactionDefault);
            }
        }
    }

    // Llamar desde Board.cs cuando se limpian líneas
    public void OnLinesCleared(int lineCount)
    {
        Sprite reaction = reactionDefault;

        switch (lineCount)
        {
            case 1:
                reaction = reaction1Line;
                break;
            case 2:
                reaction = reaction2Lines;
                break;
            case 3:
                reaction = reaction3Lines;
                break;
            case 4:
                reaction = reaction4Lines;
                break;
        }

        StartCoroutine(TransitionToReaction(reaction));
        reactionTimer = reactionDuration;
    }

    // Llamar desde GameManager.cs cuando sube el nivel
    public void OnLevelUp(int newLevel)
    {
        Sprite reaction = null;

        if (newLevel == 10 && reactionLevel10 != null)
            reaction = reactionLevel10;
        else if (newLevel == 20 && reactionLevel20 != null)
            reaction = reactionLevel20;
        else if (newLevel == 30 && reactionLevel30 != null)
            reaction = reactionLevel30;
        else if (newLevel == 50 && reactionLevel50 != null)
            reaction = reactionLevel50;
        else if (newLevel == 100 && reactionLevel100 != null)
            reaction = reactionLevel100;

        if (reaction != null)
        {
            StartCoroutine(TransitionToReactionWithFlash(reaction));
            reactionTimer = reactionDuration * 1.5f; // Más tiempo para hitos importantes
        }
    }

    private void ShowReaction(Sprite sprite)
    {
        if (sprite == null) return;

        lastReaction = sprite;

        if (leftCharacter != null)
            leftCharacter.sprite = sprite;

        if (rightCharacter != null)
            rightCharacter.sprite = sprite;
    }

    // Transición suave con fade
    private IEnumerator TransitionToReaction(Sprite newSprite)
    {
        if (leftCanvasGroup != null && rightCanvasGroup != null)
        {
            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                leftCanvasGroup.alpha = alpha;
                rightCanvasGroup.alpha = alpha;
                yield return null;
            }

            // Cambiar imagen
            ShowReaction(newSprite);

            // Fade in
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                leftCanvasGroup.alpha = alpha;
                rightCanvasGroup.alpha = alpha;
                yield return null;
            }

            leftCanvasGroup.alpha = 1f;
            rightCanvasGroup.alpha = 1f;
        }
        else
        {
            ShowReaction(newSprite);
        }
    }

    // Transición con destello para hitos importantes
    private IEnumerator TransitionToReactionWithFlash(Sprite newSprite)
    {
        if (leftCanvasGroup != null && rightCanvasGroup != null)
        {
            // Destello: más brillante
            float elapsed = 0f;
            float flashDuration = 0.2f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float brightness = Mathf.Lerp(1f, 1.5f, elapsed / flashDuration);
                if (leftCharacter != null)
                    leftCharacter.color = new Color(brightness, brightness, brightness);
                if (rightCharacter != null)
                    rightCharacter.color = new Color(brightness, brightness, brightness);
                yield return null;
            }

            // Cambiar imagen con destello visible
            ShowReaction(newSprite);

            // Volver a normal
            elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float brightness = Mathf.Lerp(1.5f, 1f, elapsed / flashDuration);
                if (leftCharacter != null)
                    leftCharacter.color = new Color(brightness, brightness, brightness);
                if (rightCharacter != null)
                    rightCharacter.color = new Color(brightness, brightness, brightness);
                yield return null;
            }

            if (leftCharacter != null)
                leftCharacter.color = Color.white;
            if (rightCharacter != null)
                rightCharacter.color = Color.white;
        }
        else
        {
            ShowReaction(newSprite);
        }
    }
}

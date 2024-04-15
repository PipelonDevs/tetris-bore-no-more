using UnityEngine;
using UnityEngine.UI;

public class ImageTransition : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;
    public float timeBetweenChanges = 18.0f;

    private float fadeDuration = 1.0f;

    private void Start()
    {
        StartCoroutine(TransitionImages());
    }

    private System.Collections.IEnumerator TransitionImages()
    {
     
        // shuffle the sprites
       


        while (true)
        {
            yield return FadeImage(false);

            image.sprite = sprites[Random.Range(0,sprites.Length-1) ];

            AdjustImageSize();

            yield return FadeImage(true);

            yield return new WaitForSeconds(timeBetweenChanges);
        }
    }

    private void AdjustImageSize()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        image.rectTransform.sizeDelta = canvasRectTransform.sizeDelta *0.6f;
        image.rectTransform.anchoredPosition = Vector2.zero;
    }

    private System.Collections.IEnumerator FadeImage(bool fadeIn)
    {
        float timer = 0f;
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, fadeIn ? 1f : 0f);

        while (timer < fadeDuration)
        {
            image.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        image.color = endColor;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CellV1 : MonoBehaviour
{
    public Vector3 originalScale;
    protected Color originalColor;
    [SerializeField] Image background;
    [SerializeField] RectTransform rectTransform;

    void Awake()
    {
        originalScale = transform.localScale;
        originalColor = background.color;
    }

    public void ResetColor()
    {
        background.color = originalColor;
    }

    public void SetAlpha(bool status)
    {
        if (status)
        {
            background.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        }
        else
        {
            background.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        }
    }

    public void SetImageTransparency(float alpha)
    {
        if (alpha > 1)
        {
            alpha = Mathf.Clamp(alpha / 255f, 0f, 1f);
        }
        else
        {
            alpha = Mathf.Clamp01(alpha);
        }

        Color color = background.color;
        color.a = alpha;
        background.color = color;
    }

    public void SetColor(Color color)
    {
        color.a = originalColor.a;
        background.color = color;
    }

    public Color GetColor()
    {
        return background.color;
    }

    public void SetBoxSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
    }

    public void SetWidth(float width)
    {
        var original = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(width, original.y);
    }

    public void SetHeight(float height)
    {
        var original = rectTransform.sizeDelta;
        rectTransform.sizeDelta = new Vector2(original.x, height);
    }

    public Vector2 GetSizeDelta()
    {
        return rectTransform.sizeDelta;
    }

    public void UpdateOriginalColor(Color color)
    {
        originalColor = color;
        background.color = originalColor;
    }
}

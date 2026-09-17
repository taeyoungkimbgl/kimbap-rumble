using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellV0 : MonoBehaviour
{
    public Vector3 originalScale;
    Color originalColor;
    Image background;
    RectTransform rectTransform;
    bool initialized;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        if (!initialized)
        {
            originalScale = transform.localScale;
            background = GetComponent<Image>();
            originalColor = background.color;
            rectTransform = GetComponent<RectTransform>();
            initialized = true;
        }
    }

    public void ResetColor()
    {
        Initialize();
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
        Initialize();

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

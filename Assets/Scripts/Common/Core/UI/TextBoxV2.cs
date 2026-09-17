using TMPro;
using UnityEngine;

public class TextBoxV2 : CellV0
{
    public TextMeshProUGUI Text;
    public GameObject LineUp;
    public GameObject LineRight;
    public string id;

    public void SetText(string text)
    {
        Text.text = text;
    }

    public string GetText()
    {
        return Text.text;
    }
    public void SetTextColor(Color color)
    {
        Text.color = color;
    }

    public void SetAlignment(TextAlignmentOptions options)
    {
        Text.alignment = options;
    }

    public void SetFixedFontSize(int textSize)
    {
        Text.enableAutoSizing = false;
        Text.fontSize = textSize;
    }

    public void SetLineUp(bool isActive)
    {
        LineUp.SetActive(isActive);
    }

    public void SetLineRight(bool isActive)
    {
        LineRight.SetActive(isActive);
    }

    public void SetWordWrapping(bool isEnableWrapping)
    {
        Text.enableWordWrapping = isEnableWrapping;
    }
}


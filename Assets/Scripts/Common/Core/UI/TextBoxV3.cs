using Assets.Scripts.Common.UI.Base;
using TMPro;
using UnityEngine;

public class TextBoxV3 : CellV1
{
    [SerializeField] protected TextMeshProUGUI textMesh;
    public string id;

    public void SetTextColor(Color color)
    {
        textMesh.color = color;
    }

    public void SetAlignment(TextAlignmentOptions options)
    {
        textMesh.alignment = options;
    }

    public void SetFixedFontSize(int textSize)
    {
        textMesh.enableAutoSizing = false;
        textMesh.fontSize = textSize;
    }

    public void SetWordWrapping(bool isEnableWrapping)
    {
        textMesh.enableWordWrapping = isEnableWrapping;
    }
}

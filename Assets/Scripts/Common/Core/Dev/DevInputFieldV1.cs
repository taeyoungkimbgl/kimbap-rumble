using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DevInputFieldTypeV1
{
    OtherText,
    TextureResizeWidth,
    LimitAngle,
    MoveSpeed,
    RotationSpeed,
    ThresholdCount,
    FingerThresholdTime,
    SkipThresholdTime,
    // YoloScaleX,
    // YoloScaleY,
}

public class DevInputFieldV1 : MonoBehaviour
{
    public DevInputFieldTypeV1 m_DevInputType;
    TMP_InputField _inputField;
    public void Initialize()
    {
        _inputField = GetComponent<TMP_InputField>();
        InitializeData(m_DevInputType);
    }

    void InitializeData(DevInputFieldTypeV1 type)
    {
        string text = "";
        switch (type)
        {
            case DevInputFieldTypeV1.OtherText:
                text = "dst";
                break;
            case DevInputFieldTypeV1.TextureResizeWidth:
                text = "0.65";
                break;
            case DevInputFieldTypeV1.LimitAngle:
                text = "3";
                break;
            case DevInputFieldTypeV1.MoveSpeed:
                text = "0.001";
                break;
            case DevInputFieldTypeV1.RotationSpeed:
                text = "0";
                break;
            case DevInputFieldTypeV1.ThresholdCount:
                text = "0.05";
                break;
            case DevInputFieldTypeV1.FingerThresholdTime:
                text = "10";
                break;
            case DevInputFieldTypeV1.SkipThresholdTime:
                text = "50";
                break;
        }
        _inputField.text = text;
    }

    public DevInputFieldTypeV1 GetInputType()
    {
        return m_DevInputType;
    }

    public TMP_InputField GetInputField()
    {
        return _inputField;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DevButtonV1 : MonoBehaviour
{
    public DevButtonTypeV1 m_BtnType;
    Button buttton;
    Image image;

    public void Initialize()
    {
        buttton = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public DevButtonTypeV1 ButtonType()
    {
        return m_BtnType;
    }

    public void PushButton()
    {
        Debug.Log("PushedButton");
        buttton.onClick.Invoke();
    }

    public void AddListener(UnityAction action)
    {
        buttton.onClick.AddListener(action);
    }

    public void SetColor(bool status)
    {
        if (status)
        {
            image.color = Color.blue;
        }
        else
        {
            image.color = Color.gray;
        }

    }
}

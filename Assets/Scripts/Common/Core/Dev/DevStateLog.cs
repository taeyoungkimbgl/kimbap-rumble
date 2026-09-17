using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DevStateLog : MonoBehaviour
{
    public GameObject TextsParent;
    TextMeshProUGUI[] texts;
    StringBuilder sb = new StringBuilder();

    public void Initialize()
    {
        texts = TextsParent.GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void ShowText(int order, string message)
    {
        texts[order].text = message;
    }

    public void ShowToStateTransformLog(int order, string objName, Vector3 position, Quaternion rotation)
    {
        sb.Length = 0;
        sb.Append("objName: ");
        sb.Append(objName);
        sb.Append(" => pos: ");
        sb.Append(position);
        sb.Append(" || rot: ");
        sb.Append(rotation);
        ShowText(order, sb.ToString());
    }

    public void ShowDebugTransformLog(string objName, Vector3 position, Quaternion rotation)
    {
        sb.Length = 0;
        sb.Append("objName: ");
        sb.Append(objName);
        sb.Append(" => pos: ");
        sb.Append(position);
        sb.Append(" || rot: ");
        sb.Append(rotation);
        // Debug.Log(_sb.ToString());
    }

}

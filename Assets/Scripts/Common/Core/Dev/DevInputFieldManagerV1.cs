using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DevInputFieldManagerV1 : MonoBehaviour
{
    public GameObject m_InputFieldParent;
    List<DevInputFieldV1> _list;

    public IEnumerator Initialize()
    {
        //GetComponentsInChildrenはActive状態のObjectのみ取得
        _list = m_InputFieldParent.transform.GetComponentsInChildren<DevInputFieldV1>().ToList();
        foreach (var item in _list)
        {
            item.Initialize();
        }
        yield return null;
    }

    public void SetData(DevInputFieldTypeV1 type, string value)
    {
        Target(type).text = value;
    }

    public string GetData(DevInputFieldTypeV1 type)
    {
        return Target(type).text;
    }

    public float GetDataf(DevInputFieldTypeV1 type)
    {
        return float.Parse(Target(type).text);
    }

    public int GetDataI(DevInputFieldTypeV1 type)
    {
        return int.Parse(Target(type).text);
    }

    public TMP_InputField GetInputField(DevInputFieldTypeV1 type)
    {
        return Target(type);
    }

    TMP_InputField Target(DevInputFieldTypeV1 type)
    {
        return _list.Find(n => n.GetInputType() == type).GetInputField();
    }

}

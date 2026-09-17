using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum DevButtonTypeV1
{
    SetActiveDebugLog,
    SetActiveStateLog,
    SetActiveDropdowns,
    SetActiveInputFields,
    SetActiveDevButtons,
    ClearDebugLog,
    SetActiveAllPanels,
    Reset,
}

public class DevButtonManagerV1 : MonoBehaviour
{
    public DevButtonV1 m_DisplayButton;
    public GameObject m_ButtonParent;
    List<DevButtonV1> _list;

    public IEnumerator Initialize()
    {
        //GetComponentsInChildrenはActive状態のObjectのみ取得
        _list = m_ButtonParent.transform.GetComponentsInChildren<DevButtonV1>().ToList();
        foreach (var button in _list)
        {
            button.Initialize();
        }
        m_DisplayButton.Initialize();
        yield return null;
    }

    public void PushButton(DevButtonTypeV1 type)
    {
        Target(type).PushButton();
    }

    public void AddListener(DevButtonTypeV1 type, UnityAction action)
    {
        Target(type).AddListener(action);
    }

    public void SetColor(DevButtonTypeV1 type, bool status)
    {
        Target(type).SetColor(status);
    }

    DevButtonV1 Target(DevButtonTypeV1 type)
    {
        return _list.Find(n => n.ButtonType() == type);
    }
}

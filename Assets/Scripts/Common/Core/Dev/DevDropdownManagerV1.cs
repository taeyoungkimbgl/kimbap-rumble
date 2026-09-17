using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DevDropdownManagerV1 : MonoBehaviour
{
    public GameObject m_DropdownParent;
    List<DevDropdownV1> _list;

    public IEnumerator Initialize()
    {
        _list = m_DropdownParent.transform.GetComponentsInChildren<DevDropdownV1>().ToList();
        foreach (var item in _list)
        {
            item.Initialize();
        }
        yield return null;
    }

    public void SetOption(DevDropdownTypeV1 type, string[] names)
    {
        Target(type).GetComponent<DevDropdownV1>().SetOptions(names);
    }

    public void SetValue(DevDropdownTypeV1 type, int value)
    {
        Target(type).value = value;
    }

    public int GetValue(DevDropdownTypeV1 type)
    {
        return Target(type).value;
    }

    public string GetOptionName(DevDropdownTypeV1 type)
    {
        var target = Target(type);
        var value = target.value;
        return target.options[value].text;
    }

    public DevDropdownV1 GetDevDropdown(DevDropdownTypeV1 type)
    {
        return _list.Find(n => n.GetDropdownType() == type);
    }

    public TMP_Dropdown GetDropdown(DevDropdownTypeV1 type)
    {
        return Target(type);
    }

    TMP_Dropdown Target(DevDropdownTypeV1 type)
    {
        return _list.Find(n => n.GetDropdownType() == type).GetDropdown();
    }
}

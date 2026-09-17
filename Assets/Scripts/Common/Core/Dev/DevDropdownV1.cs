using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static TMPro.TMP_Dropdown;


public enum DevDropdownTypeV1
{
    OperationTicket,
    Type1,
}

public class DevDropdownV1 : MonoBehaviour
{
    public DevDropdownTypeV1 m_DevDropdownType;
    TMP_Dropdown _dropdown;
    string[] _enumNames;
    List<OptionData> _optionDataList;

    public void Initialize()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _optionDataList = new List<OptionData>();
        InitializeData(m_DevDropdownType);
    }

    void InitializeData(DevDropdownTypeV1 type)
    {
        switch (type)
        {
            case DevDropdownTypeV1.OperationTicket:
                break;
        }

    }

    public void SetOptions(string[] names)
    {
        _dropdown.ClearOptions();
        _optionDataList.Clear();
        for (int i = 0; i < names.Length; i++)
        {
            var optionData = new OptionData();
            optionData.text = names[i];
            _optionDataList.Add(optionData);
        }
        _dropdown.AddOptions(_optionDataList);
    }

    public DevDropdownTypeV1 GetDropdownType()
    {
        return m_DevDropdownType;
    }

    public TMP_Dropdown GetDropdown()
    {
        return _dropdown;
    }

    public DevDropdownV1 GetDevDropdown()
    {
        return this;
    }
}

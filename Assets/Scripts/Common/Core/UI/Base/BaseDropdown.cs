using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Events;
using static TMPro.TMP_Dropdown;

namespace Assets.Scripts.Common.UI.Base
{
    public class BaseDropdown<T> : BaseUI<T> where T : Enum
    {
        protected TMP_Dropdown dropdown;
        private Dictionary<string, string> dropdownList;


        protected void Awake()
        {
            dropdown = GetComponents<TMP_Dropdown>()?[0];
        }

        public void UpdateDropdown(Dictionary<string, string> list, string firstKey)
        {
            dropdownList = list;
            dropdown.ClearOptions();
            CreateDropdown(firstKey);
        }

        private void CreateDropdown(string firstKey)
        {
            if (dropdownList.Count <= 0)
            {
                return;
            }

            var optionDataList = new List<OptionData>();
            foreach (var item in dropdownList.Values)
            {
                OptionData newData = new()
                {
                    text = item
                };

                optionDataList.Add(newData);
            }

            dropdown.AddOptions(optionDataList);

            var keys = dropdownList.Keys.ToList();
            var index = keys.IndexOf(firstKey);

            dropdown.value = index;

        }

        public void SetOnValueChangedListener(Action action)
        {
            dropdown.onValueChanged.AddListener(delegate { action(); });
        }

        public void SetOnValueChangedListener(UnityAction<int> call)
        {
            dropdown.onValueChanged.AddListener(call);
        }

        public string GetSelectedValue()
        {
            return dropdown.options[dropdown.value].text;
        }

        public string GetSelectedKey()
        {
            var value = dropdown.options[dropdown.value].text;
            return dropdownList.FirstOrDefault(o => o.Value == value).Key.ToString();
        }

        public void AddOptions(string[] names)
        {
            var options = new List<OptionData>();

            for (int i = 0; i < names.Length; i++)
            {
                OptionData newData = new OptionData();
                newData.text = names[i];
                options.Add(newData);
            }
            dropdown.AddOptions(options);
        }

    }
}
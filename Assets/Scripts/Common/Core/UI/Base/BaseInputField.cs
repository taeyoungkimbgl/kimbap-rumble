using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Common.UI.Base
{
    public class BaseInputField<T> : BaseUI<T> where T : Enum
    {
        protected TMP_InputField _inputField;

        void Awake()
        {
            _inputField = GetComponents<TMP_InputField>()?[0];
        }

        public void SetInputText(string text)
        {
            if (_inputField == null)
            {
                _inputField = GetComponents<TMP_InputField>()?[0];
            }
            _inputField.text = text;
        }

        public string GetInputValue()
        {
            return _inputField.text;
        }

        public void SetInteractable(bool isTnteractable)
        {
            _inputField.interactable = isTnteractable;
        }

    }
}
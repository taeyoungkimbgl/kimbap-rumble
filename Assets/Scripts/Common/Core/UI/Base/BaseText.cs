using System;
using TMPro;

namespace Assets.Scripts.Common.UI.Base
{
    public class BaseText<T> : BaseUI<T> where T : Enum
    {
        TextMeshProUGUI textMesh;

        protected virtual void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }


        public string GetText()
        {
            return textMesh.text;
        }


        public void SetText(string text)
        {
            if (textMesh == null)
            {
                textMesh = GetComponent<TextMeshProUGUI>();
            }

            textMesh.text = text;
        }

    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.UI.Base
{
    [RequireComponent(typeof(Image))]
    public class BaseImage<T> : BaseUI<T> where T : Enum
    {
        private Image _image;

        protected void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            _image.color = color;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}

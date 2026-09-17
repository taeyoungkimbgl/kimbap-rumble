using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Common.UI.Base
{
    public class BaseButton<T> : BaseText<T>, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler where T : Enum
    {
        public bool EnableHighlight = true;
        public virtual bool IsSetBtnKey => true;

        Action onClick;
        Image buttonImage;
        Color originalColor;

        bool isPressed;

        protected override void Awake()
        {
            base.Awake();
            IsEnabled = true;

            if (IsSetBtnKey)
            {
                gameObject.name = Key.ToString();
            }
            buttonImage = GetComponent<Image>();
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public bool IsEnabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                SetActiveView(enabled);
            }
        }

        public void SetListener(Action action)
        {
            if (onClick != null)
            {
                RemoveAllListeners();
            }

            onClick = action;
        }

        public void RemoveAllListeners()
        {
            onClick = null;
        }

        public void OnClick()
        {
            onClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            if (EnableHighlight && buttonImage != null)
            {
                buttonImage.color = originalColor * 0.9f;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (EnableHighlight && buttonImage != null)
            {
                buttonImage.color = originalColor;
            }

            isPressed = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPressed) return;
            isPressed = false;
            if (EnableHighlight && buttonImage != null)
            {
                buttonImage.color = originalColor;
            }
        }

        void SetActiveView(bool isActive)
        {
            foreach (Transform childTransform in GetComponentsInChildren<Transform>())
            {

                if (childTransform.GetComponent<TMP_Text>() == null)
                {
                    continue;
                }

                if (!childTransform.TryGetComponent<CanvasGroup>(out var canvasGroup))
                {
                    canvasGroup = childTransform.gameObject.AddComponent<CanvasGroup>();
                }

                canvasGroup.alpha = isActive ? 1.0f : 0.2f;
                canvasGroup.interactable = isActive;
                canvasGroup.blocksRaycasts = isActive;
            }
        }

        public void SetColor(Color color)
        {
            color.a = originalColor.a;
            originalColor = color;
            buttonImage.color = color;
        }


    }
}

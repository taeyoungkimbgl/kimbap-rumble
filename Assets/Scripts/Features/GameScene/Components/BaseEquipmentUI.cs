using System;
using Assets.Scripts.Common.UI.Base;
using Scripts.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Features.GameScene.Components
{
    public class BaseEquipmentUI<T> : BaseUI<T>, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler where T : Enum
    {
        [SerializeField] TextMeshProUGUI _textMesh;
        [SerializeField] Image _image;
        [FormerlySerializedAs("_equipementId")]
        [SerializeField] string _equipmentId;
        [SerializeField] EquipmentType _equipmentType;

        Action<string, EquipmentType> _onClick;

        public string EquipmentId => _equipmentId;
        public EquipmentType EquipmentType => _equipmentType;

        public bool EnableHighlight = true;
        public virtual bool IsSetBtnKey => true;

        Image _buttonImage;
        Color _originalColor;

        bool isPressed;

        protected virtual void Awake()
        {
            IsEnabled = true;

            if (IsSetBtnKey)
            {
                gameObject.name = Key.ToString();
            }
            _buttonImage = GetComponent<Image>();
            if (_buttonImage != null)
            {
                _originalColor = _buttonImage.color;
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

        public void SetListener(Action<string, EquipmentType> action)
        {
            _onClick = action;
        }

        public void RemoveAllListeners()
        {
            _onClick = null;
        }

        public void OnClick()
        {
            NotifyClick();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            if (EnableHighlight && _buttonImage != null)
            {
                _buttonImage.color = _originalColor * 0.9f;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (EnableHighlight && _buttonImage != null)
            {
                _buttonImage.color = _originalColor;
            }

            isPressed = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isPressed) return;
            isPressed = false;
            if (EnableHighlight && _buttonImage != null)
            {
                _buttonImage.color = _originalColor;
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
            color.a = _originalColor.a;
            _originalColor = color;
            _buttonImage.color = color;
        }

        public string GetText() => _textMesh.text;
        public void SetText(string text) => _textMesh.text = text;

        public Sprite GetSprite() => _image.sprite;
        public void SetSprite(Sprite sprite) => _image.sprite = sprite;

        public void SetEquipmentId(string equipmentId) => _equipmentId = equipmentId;
        public void SetEquipmentType(EquipmentType equipmentType) => _equipmentType = equipmentType;

        void NotifyClick()
        {
            _onClick?.Invoke(_equipmentId, _equipmentType);
        }
    }
}

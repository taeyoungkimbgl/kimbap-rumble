using System;
using Assets.Scripts.Common.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Features.GameScene.Header
{
    public class HeaderView : BaseView
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Button _backButton;

        public event Action BackClicked;

        protected override void Awake()
        {
            base.Awake();
            if (_title == null || _backButton == null)
                throw new InvalidOperationException("HeaderView requires Title and BackButton references.");

            Render(string.Empty, false, false);
            _backButton.onClick.AddListener(OnBackClicked);
        }

        public void Render(string title, bool backVisible, bool inputEnabled)
        {
            _title.text = title;
            _backButton.gameObject.SetActive(backVisible);
            _backButton.interactable = backVisible && inputEnabled;
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
        }

        private void OnDestroy()
        {
            // The separately owned Canvas may already have been destroyed on scene unload.
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);
        }
    }
}

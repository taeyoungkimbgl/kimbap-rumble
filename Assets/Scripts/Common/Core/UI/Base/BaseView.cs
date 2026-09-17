using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Scripts.Common.UI.Base
{
    public abstract class BaseView : MonoBehaviour
    {
        [FormerlySerializedAs("Canvas")]
        [SerializeField] private Canvas _canvas;

        private List<BaseUIAbstract> _uiElements;

        protected virtual void Awake()
        {
            if (_canvas == null)
                throw new InvalidOperationException($"{GetType().Name} requires a Canvas reference.");

            _uiElements = new List<BaseUIAbstract>(
                _canvas.GetComponentsInChildren<BaseUIAbstract>(true));
        }

        protected TUI GetUI<TUI, TEnum>(TEnum key) where TUI : BaseUI<TEnum> where TEnum : Enum
        {
            foreach (var ui in _uiElements)
            {
                if (ui is TUI typed && ui.Key.Equals(key))
                    return typed;
            }
            return null;
        }

        protected List<BaseUI<TEnum>> GetUIs<TUI, TEnum>() where TUI : BaseUI<TEnum> where TEnum : Enum
        {
            var results = new List<BaseUI<TEnum>>();
            foreach (var ui in _uiElements)
            {
                if (ui is TUI typed)
                {
                    results.Add(typed);
                }
            }
            return results;
        }

    }
}

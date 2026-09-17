using System;

namespace Scripts.Features.GameScene.Header
{
    public class HeaderModel
    {
        public string Title { get; private set; } = string.Empty;
        public bool BackVisible { get; private set; }
        public bool InputEnabled { get; private set; }

        public event Action Changed;
        public event Action BackRequested;

        public void Apply(string title, bool backVisible)
        {
            Title = title;
            BackVisible = backVisible;
            Changed?.Invoke();
        }

        public void SetInputEnabled(bool enabled)
        {
            InputEnabled = enabled;
            Changed?.Invoke();
        }

        public void RequestBack()
        {
            if (BackVisible && InputEnabled)
                BackRequested?.Invoke();
        }
    }
}

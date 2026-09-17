using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common.UI.Base
{
    public class BaseCell : MonoBehaviour
    {
        Color originalColor;
        Image background;

        void Awake()
        {
            background = GetComponent<Image>();
            originalColor = background.color;
        }

        public void ResetColor()
        {
            background.color = originalColor;
        }

        public void SetColor(Color color)
        {
            color.a = originalColor.a;
            background.color = color;
        }
    }
}

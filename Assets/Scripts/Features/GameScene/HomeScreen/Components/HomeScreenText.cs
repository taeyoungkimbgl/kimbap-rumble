using Assets.Scripts.Common.UI.Base;
using TMPro;
using UnityEngine;

namespace Scripts.Features.GameScene.HomeScreen.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class HomeScreenText : BaseText<HomeScreenTextType>
    {
    }
    public enum HomeScreenTextType
    {
        Title = 0,
        Character = 1,
        WeaponHeading = 2,
        ShieldHeading = 3,
        AccessoryHeading = 4
    }
}

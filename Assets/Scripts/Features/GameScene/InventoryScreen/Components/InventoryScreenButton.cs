using Assets.Scripts.Common.UI.Base;
using UnityEngine;

namespace Scripts.Features.GameScene.InventoryScreenScreen.Components
{
    public class InventoryScreenButton : BaseButton<InventoryScreenButtonType> { }

    public enum InventoryScreenButtonType
    {
        Back = 0,
        Equip = 1,
    }
}

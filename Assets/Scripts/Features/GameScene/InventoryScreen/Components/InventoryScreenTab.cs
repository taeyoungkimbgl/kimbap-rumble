using Assets.Scripts.Common.UI.Base;
using UnityEngine;

namespace Scripts.Features.GameScene.InventoryScreenScreen.Components
{
    public class InventoryScreenTab : BaseTab<InventoryScreenTabType>
    {
    }

    public enum InventoryScreenTabType
    {
        Weapon = 0,
        Shield = 1,
        Accessory = 2,
    }
}

using Scripts.Constants;
using Scripts.Features.GameScene.Components;
using UnityEngine;

namespace Scripts.Features.GameScene.InventoryScreen.Components
{
    public class EquipmentCard : BaseEquipmentUI<EquipmentType>
    {
        [SerializeField] private GameObject _selectionFrame;

        public bool IsSelected => _selectionFrame.activeSelf;

        public void SetSelected(bool selected)
        {
            _selectionFrame.SetActive(selected);
        }
    }
}

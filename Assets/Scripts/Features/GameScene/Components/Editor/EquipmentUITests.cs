using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Scripts.Constants;
using Scripts.Features.GameScene.HomeScreen.Components;
using Scripts.Features.GameScene.InventoryScreen.Components;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Scripts.Features.GameScene.Components.Editor
{
    public class EquipmentUITests
    {
        [UnityTest]
        public IEnumerator ReusedCardsAndSlotsNotifyCurrentEquipmentAndKeepSelectionIndependent()
        {
            yield return new EnterPlayMode();
            VerifyReuse();
            yield return null;
            yield return new ExitPlayMode();
        }

        private static void VerifyReuse()
        {
            var root = new GameObject("Equipment UI test", typeof(EventSystem));
            var pointer = new PointerEventData(root.GetComponent<EventSystem>());
            var template = CreateUI<EquipmentCard>(root.transform);
            var frame = new GameObject("SelectionFrame", typeof(RectTransform));
            frame.transform.SetParent(template.transform, false);
            frame.SetActive(false);
            Assign(template, "_selectionFrame", frame);
            template.SetEquipmentType(EquipmentType.Weapon);

            var texture = new Texture2D(2, 2);
            var sprite = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
            var notifications = new List<(string id, EquipmentType type)>();
            var cards = new List<EquipmentCard>();

            for (var i = 0; i < 20; i++)
            {
                var card = Object.Instantiate(template, root.transform);
                card.SetEquipmentId("weapon_" + i);
                card.SetText("Weapon " + i);
                card.SetSprite(sprite);
                card.SetListener((id, type) => notifications.Add((id, type)));
                card.gameObject.SetActive(true);
                cards.Add(card);
                Assert.AreEqual("Weapon " + i, card.GetText());
                Assert.AreSame(sprite, card.GetSprite());
                Assert.IsFalse(card.IsSelected);
                ExecuteEvents.Execute(card.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                Assert.AreEqual(("weapon_" + i, EquipmentType.Weapon), notifications[i]);
            }

            cards[0].SetSelected(true);
            Assert.IsTrue(cards[0].IsSelected);
            Assert.IsFalse(cards[1].IsSelected);
            Assert.IsTrue(cards[0].IsEnabled);
            Assert.AreEqual(20, notifications.Count, "Selection must not emit a click.");

            var reused = cards[0];
            for (var i = 0; i < 3; i++)
            {
                reused.gameObject.SetActive(false);
                reused.SetEquipmentId("shield_" + i);
                reused.SetEquipmentType(EquipmentType.Shield);
                reused.SetText("Shield " + i);
                reused.SetSelected(false);
                reused.gameObject.SetActive(true);
                ExecuteEvents.Execute(reused.gameObject, pointer, ExecuteEvents.pointerClickHandler);
                Assert.AreEqual("Shield " + i, reused.GetText());
                Assert.AreEqual(21 + i, notifications.Count, "Enable cycles must not duplicate listeners.");
                Assert.AreEqual(("shield_" + i, EquipmentType.Shield), notifications[20 + i]);
                Assert.IsFalse(reused.IsSelected);
            }

            reused.enabled = false;
            ExecuteEvents.Execute(reused.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            Assert.AreEqual(23, notifications.Count);
            reused.enabled = true;
            var replacements = 0;
            reused.SetListener((id, type) => replacements++);
            ExecuteEvents.Execute(reused.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            Assert.AreEqual(1, replacements);
            reused.OnClick();
            Assert.AreEqual(2, replacements);
            Assert.AreEqual(23, notifications.Count);
            reused.RemoveAllListeners();
            ExecuteEvents.Execute(reused.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            reused.OnClick();
            Assert.AreEqual(2, replacements);

            var slot = CreateUI<EquipmentSlot>(root.transform);
            slot.SetEquipmentType(EquipmentType.Accessory);
            slot.SetEquipmentId("accessory_01");
            slot.SetText("Ribbon");
            slot.SetSprite(sprite);
            slot.SetListener((id, type) => notifications.Add((id, type)));
            slot.gameObject.SetActive(true);
            ExecuteEvents.Execute(slot.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            Assert.AreEqual(("accessory_01", EquipmentType.Accessory), notifications[23]);
            Assert.AreEqual("Ribbon", slot.GetText());
            Assert.AreSame(sprite, slot.GetSprite());
            Assert.AreEqual("accessory_01", slot.EquipmentId);
            Assert.AreEqual(EquipmentType.Accessory, slot.EquipmentType);

            Object.Destroy(root);
            Object.Destroy(sprite);
            Object.Destroy(texture);
        }

        private static T CreateUI<T>(Transform parent) where T : BaseEquipmentUI<EquipmentType>
        {
            var go = new GameObject(typeof(T).Name, typeof(RectTransform), typeof(Image));
            go.SetActive(false);
            go.transform.SetParent(parent, false);
            var ui = go.AddComponent<T>();
            var label = new GameObject("Name", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(go.transform, false);
            var icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(go.transform, false);
            Assign(ui, "_textMesh", label.GetComponent<TextMeshProUGUI>());
            Assign(ui, "_image", icon.GetComponent<Image>());
            return ui;
        }

        private static void Assign(Object target, string field, Object value)
        {
            var serialized = new SerializedObject(target);
            serialized.FindProperty(field).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}

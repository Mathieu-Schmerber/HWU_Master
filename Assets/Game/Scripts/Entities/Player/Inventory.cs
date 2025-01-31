﻿using Game.Managers;
using Game.Systems.Items;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Player
{
	public class Inventory : MonoBehaviour
	{
		#region Types

		public class InventoryUpdateEventArgs : EventArgs
		{
			public enum UpdateType
			{
				UPGRADE,
				DROP,
				MERGE,
				EQUIP,
				GLOBAL_UPDATE
			}

			public Inventory inventory;
			public UpdateType Event;

			public InventoryUpdateEventArgs(Inventory inventory, UpdateType @event)
			{
				this.inventory = inventory;
				Event = @event;
			}
		}

		[Serializable] public class ItemDictionary : SerializedDictionary<ItemBaseData, AEquippedItem> { }
		#endregion

		[SerializeField] private GameObject _inventoryParent;
		[ShowInInspector, ReadOnly] private ItemDictionary _items = new();
		private int _slotNumber;
		private int _occupiedSlots = 0;

		public AEquippedItem[] Items => _items.Where(x => x.Value != null).Select(x => x.Value).ToArray();
		public bool HasAvailableSlot => _occupiedSlots < _slotNumber;

		public static event Action<InventoryUpdateEventArgs> OnUpdated;

		#region Unity Builtins

		private void Awake()
		{
			_slotNumber = Databases.Database.Data.Item.Settings.InventorySlots;
		}

		#endregion

		public void RemoveAllItems()
		{
			_items.ToList().ForEach(x => RemoveItemFromInventory(x.Key));
			OnUpdated?.Invoke(new(this, InventoryUpdateEventArgs.UpdateType.GLOBAL_UPDATE));
		}

		private void AddItemToInventory(ItemSummary item)
		{
			AEquippedItem behaviour = _inventoryParent.AddComponent(item.Data.Component) as AEquippedItem;

			_occupiedSlots++;
			_items[item.Data] = behaviour;
			behaviour.OnEquipped(item);

			if (item.isMerged)
			{
				AEquippedItem mergedBehaviour = _inventoryParent.AddComponent(item.Merge.Data.Component) as AEquippedItem;
				mergedBehaviour.OnEquipped(item.Merge);
				behaviour.MergedBehaviour = mergedBehaviour;
			}
		}

		public void RemoveItemFromInventory(ItemBaseData item, bool updateInventory = false)
		{
			ItemSummary summary = _items[item].Summary;

			_occupiedSlots--;
			if (summary.isMerged)
				_items[item].MergedBehaviour.OnUnequipped();
			_items[item].OnUnequipped();
			_items.Remove(_items[item].Details);
			if (updateInventory)
				OnUpdated?.Invoke(new(this, InventoryUpdateEventArgs.UpdateType.DROP));
		}

		#region Public access

		public bool HasEquipped(ItemBaseData item) => 
			_items.ContainsKey(item) && // The inventory has the item key
			_items[item] != null &&		// The inventory has the item value
			!_items.Values.Any(x => x.Summary.isMerged && x.Summary.Merge.Data == item); // No merged item contains the item

		public void EquipItem(ItemSummary summary)
		{
			if (!HasAvailableSlot)
				return;

			AEquippedItem exists = _items.ContainsKey(summary.Data) ? _items[summary.Data] : null;

			if (exists == null)
			{
				AddItemToInventory(summary);
				OnUpdated?.Invoke(new(this, InventoryUpdateEventArgs.UpdateType.EQUIP));
			}
		}

		public ItemBaseData GetRandomUnEquippedItem(bool includeLifeItem)
			=> Databases.Database.Data.Item.All<ItemBaseData>().Where(x => x.IsLifeItem == includeLifeItem && !HasEquipped(x)).Random();

		public static void OnItemUpgraded() => OnUpdated?.Invoke(new(GameManager.Player.GetComponent<Inventory>(), InventoryUpdateEventArgs.UpdateType.UPGRADE));

		public void DropItem(ItemBaseData item)
		{
			LootedItem.Create(transform.position, _items[item].Summary);
			RemoveItemFromInventory(item);
			OnUpdated?.Invoke(new(this, InventoryUpdateEventArgs.UpdateType.DROP));
		}

		public bool TryMergeItems(AEquippedItem a, AEquippedItem b)
		{
			int cost = GetMergeCost(a, b);

			if (!CanBeMerged(a, b) || !GameManager.CanRunMoneyAfford(cost))
				return false;

			// Set merged state
			a.Summary.Merge = b.Summary;
			a.MergedBehaviour = b;

			// Remove b's data from inventory, without removing its behaviour
			_occupiedSlots--;
			_items.Remove(b.Details);
			OnUpdated?.Invoke(new(this, InventoryUpdateEventArgs.UpdateType.MERGE));

			GameManager.PayWithRunMoney(cost);
			return true;
		}

		public static bool CanBeMerged(AEquippedItem a) => !a.Details.IsLifeItem && !a.Summary.isMerged;

		public static bool CanBeMerged(AEquippedItem a, AEquippedItem b)
		{
			AEquippedItem[] equips = { a, b };

			return CanBeMerged(a) && CanBeMerged(b) && equips.Any(x => x.Details.Type == ItemType.STAT);
		}

		public static int GetMergeCost(AEquippedItem a, AEquippedItem b) => a.NextUpgradePrice + b.NextUpgradePrice;

		#endregion
	}
}

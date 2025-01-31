﻿using Game.Entities.Player;
using Game.Managers;
using Game.Tools;
using Nawlian.Lib.Extensions;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ItemMergeMenuUi : ACloseableMenu
	{
		[Title("References")]
		[SerializeField] private Transform _globalPanel;
		[SerializeField] private Transform _emptyPanel;
		[SerializeField] private SimpleDescriptorUi _resultItem;
		[SerializeField] private TextMeshProUGUI _priceTxt;
		[SerializeField] private Transform _interactionBox;
		[SerializeField] private InventorySlotUi[] _additionSlots;
		[SerializeField] private Button _purchaseBtn;

		[Title("Feedback")]
		[SerializeField] private Color _flashColor;
		[SerializeField] private float _bumpIntensity;
		[SerializeField] private AudioClip _errorAudio;

		private Color _resultDefaultColor;
		private Image _resultRenderer;
		private int _slotIndex;
		private Inventory _inventory;
		private InventorySlotUi _lastSelectedSlot;

		#region Properties

		private InventorySlotUi CurrentSlot {
			get
			{
				if (_slotIndex < 0)
					_slotIndex = _additionSlots.Length - 1;
				else if (_slotIndex >= _additionSlots.Length)
					_slotIndex = 0;
				return _additionSlots[_slotIndex];
			}
		}

		private bool _isAdditionReady => !_additionSlots.Any(x => x.IsEmpty);

		private bool _isAdditionCorrect => _isAdditionReady && Inventory.CanBeMerged(_additionSlots[0].Item, _additionSlots[1].Item);

		#endregion

		protected override void Awake()
		{
			base.Awake();
			_resultRenderer = _resultItem.GetComponent<Image>();
			_resultDefaultColor = _resultRenderer.color;
			_inventory = GameManager.Player.GetComponent<Inventory>();
		}

		private void Start()
		{
			_purchaseBtn.onClick.AddListener(OnPurchaseSubmit);
		}

		private bool TryMerging()
		{
			bool result = _inventory.TryMergeItems(_additionSlots[0].Item, _additionSlots[1].Item);

			if (!result)
				_source.PlayOneShot(_errorAudio);
			return result;
		}
		

		#region Slot handling

		private void ResetAddition()
		{
			_additionSlots.ForEach(x => SetCurrentSlot(null));
			_slotIndex = 0;
			RefreshUi();
		}

		private void SetCurrentSlot(InventorySlotUi slot)
		{
			CurrentSlot.SetItem(slot?.Item);

			// Bump
			Tween.LocalScale(CurrentSlot.transform, Vector3.one * _bumpIntensity, _duration / 2, 0, Tween.EaseBounce);
			Tween.LocalScale(CurrentSlot.transform, Vector3.one, _duration / 2, _duration / 2, Tween.EaseBounce);

			_slotIndex++;
		}

		private void OnPurchaseSubmit()
		{
			InventorySlotUi slot = _lastSelectedSlot;

			if (TryMerging())
			{
				Feedback();
				ResetAddition();
			}
		}

		private void OnSlotSubmitted(InventorySlotUi slot)
		{
			if (slot.IsEmpty) // Cannot do a thing with an empty slot
				return;
			else if (!_isAdditionReady && _additionSlots.Any(x => x.Item == slot.Item)) // Cannot use a duplicate
				return;
			if (_isAdditionReady)
				ResetAddition();
			SetCurrentSlot(slot);
			RefreshUi();
		}

		private void OnSlotSelected(InventorySlotUi slot)
		{
			_lastSelectedSlot = slot;
		}

		#endregion

		#region Display

		private void DisplayAdditionResult()
		{
			if (_additionSlots.Count(x => x.IsEmpty) == 2)
				_resultItem.Describe("No result", "Select two items to be merged", 0);
			else
			{
				if (_isAdditionReady || _additionSlots.Count(x => x.IsEmpty) == 1)
				{
					string description = _additionSlots[0].Item.GetDescription();

					if (!_additionSlots[1].IsEmpty)
						description = $"{description}{Environment.NewLine}{_additionSlots[1].Item.GetDescription()}";
					_resultItem.Describe(_additionSlots[0].Item.Details.name, description, _additionSlots[0].Item.Quality);
				}
				else if (!_isAdditionCorrect)
				{
					_resultItem.Describe(
						$"Incompatible items",
						$"These items cannot be merged together.{Environment.NewLine}Try another equation.",
						_additionSlots[0].Item.Quality);
				}
			}
		}

		private void DisplayInteraction() => _purchaseBtn.interactable = _isAdditionReady && _isAdditionCorrect;

		private void DisplayPrice()
		{
			if (_isAdditionCorrect)
			{
				int cost = Inventory.GetMergeCost(_additionSlots[0].Item, _additionSlots[1].Item);
				_priceTxt.text = cost.ToString();
				_priceTxt.color = GameManager.CanRunMoneyAfford(cost) ? Color.white : Color.red;
			}
			else
			{
				_priceTxt.text = "???";
				_priceTxt.color = Color.red;
			}
		}

		private void RefreshUi()
		{
			if (!InventorySlotSelector.HasUsableSlot)
			{
				_globalPanel.gameObject.SetActive(false);
				_emptyPanel.gameObject.SetActive(true);
				return;
			}
			DisplayAdditionResult();
			DisplayInteraction();
			DisplayPrice();
		}

		private void Feedback()
		{
			// Flash
			Tween.Value(_resultDefaultColor, _flashColor, (c) => _resultRenderer.color = c, _duration / 2, 0);
			Tween.Value(_flashColor, _resultDefaultColor, (c) => _resultRenderer.color = c, _duration / 2, _duration / 2);

			// Bump
			Tween.LocalScale(_interactionBox, Vector3.one * _bumpIntensity, _duration / 2, 0, Tween.EaseBounce);
			Tween.LocalScale(_interactionBox, Vector3.one, _duration / 2, _duration / 2, Tween.EaseBounce);
		}

		#endregion

		#region Open / Close

		public override void Open()
		{
			if (InventorySlotSelector.IsInUse)
				return;
			base.Open();
			
			ResetAddition();
			RefreshUi();
			InventorySlotSelector.StartUsing(x => !x.IsEmpty && Inventory.CanBeMerged(x.Item));
			InventorySlotSelector.SetRightNavigation(_purchaseBtn);
			InventorySlotUi.OnSelected += OnSlotSelected;
			InventorySlotUi.OnSubmitted += OnSlotSubmitted;

			_globalPanel.gameObject.SetActive(InventorySlotSelector.HasUsableSlot);
			_emptyPanel.gameObject.SetActive(!InventorySlotSelector.HasUsableSlot);
		}

		public override void Close()
		{
			base.Close();
			InventorySlotSelector.EndUsing();
			InventorySlotSelector.SetRightNavigation(null);
			InventorySlotUi.OnSelected -= OnSlotSelected;
			InventorySlotUi.OnSubmitted -= OnSlotSubmitted;
		}

		#endregion
	}
}

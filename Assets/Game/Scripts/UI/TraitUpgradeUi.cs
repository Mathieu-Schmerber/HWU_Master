﻿using Game.Managers;
using Game.Systems.Run.Lobby;
using Game.Tools;
using Nawlian.Lib.Extensions;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class TraitUpgradeUi : ACloseableMenu
	{
		private TraitDescriptorUi[] _descriptors;
		[SerializeField] private AudioClip _purchaseAudio;
		[SerializeField] private AudioClip _errorAudio;
		[SerializeField] private Button _purchaseButton;

		private TraitDescriptorUi _lastSelectedTrait;

		protected override void Awake()
		{
			base.Awake();
			_descriptors = GetComponentsInChildren<TraitDescriptorUi>();
		}

		private void Start()
		{
			_purchaseButton.onClick.AddListener(OnSubmitted);
		}

		private void Refresh(TraitDescriptorUi x)
		{
			TraitUpgradeData trait = x.Trait;
			int cost = GameManager.GetTraitUpgradeCost(trait);

			x.UpdatePrice(cost);
			x.SetStatsText($"{trait.StatName}: {GameManager.PlayerIdentity.Stats.Modifiers[trait.StatModified].Value}%<color=green>(+{trait.IncrementPerUpgrade}%)</color>");
			x.Interactable = GameManager.IsUpgradable(trait);
			_purchaseButton.interactable = GameManager.CanLobbyMoneyAfford(cost) && GameManager.IsUpgradable(trait);
		}

		private void OnSubmitted() => PerformPurchase(_lastSelectedTrait);

		private void SelectTrait(TraitDescriptorUi descriptorUi)
		{
			if (descriptorUi == null)
				return;
			EventSystem.current.SetSelectedGameObject(descriptorUi.gameObject);
			descriptorUi.Select();
			descriptorUi.OnSelect(null);
		}

		private void PerformPurchase(TraitDescriptorUi descriptorUi)
		{
			if (descriptorUi == null)
				return;
			else if (GameManager.UpgradeTrait(descriptorUi.Trait))
			{
				_descriptors.ForEach(x => Refresh(x));
				Refresh(descriptorUi);
				descriptorUi.Interact();
				_source.PlayOneShot(_purchaseAudio);
			}
			else
				_source.PlayOneShot(_errorAudio);
			if (!descriptorUi.Interactable)
				SelectTrait(_descriptors.FirstOrDefault(x => x.Interactable));
			else
				SelectTrait(descriptorUi);
		}

		private void OnSelected(TraitDescriptorUi descriptorUi)
		{
			int cost = GameManager.GetTraitUpgradeCost(descriptorUi.Trait);

			_lastSelectedTrait = descriptorUi;
			_purchaseButton.interactable = GameManager.CanLobbyMoneyAfford(cost) && GameManager.IsUpgradable(descriptorUi.Trait);
		}

		public override void Open()
		{
			base.Open();
			_descriptors.ForEach(x => Refresh(x));

			TraitDescriptorUi selected = _descriptors.FirstOrDefault(x => x.Interactable);

			if (selected != null)
			{
				SelectTrait(selected);
				Refresh(selected);
			}

			TraitDescriptorUi.OnTraitSelected += OnSelected;
			TraitDescriptorUi.OnTraitSubmitted += PerformPurchase;
		}

		public override void Close()
		{
			base.Close();
			EventSystem.current.SetSelectedGameObject(null);
			TraitDescriptorUi.OnTraitSelected -= OnSelected;
			TraitDescriptorUi.OnTraitSubmitted -= PerformPurchase;
		}
	}
}

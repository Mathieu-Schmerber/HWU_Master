using Game.Entities.AI;
using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Dialogue;
using Game.Systems.Items;
using Game.Systems.Run;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Interaction;
using Nawlian.Lib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
	public class CroupierDialogue : ADialogueInterpreter, IInteractable
	{
		#region Types

		public class BetInfo
		{
			public int MoneyBet { get; set; }
			public int MoneyReward => MoneyBet * 2;
			public AEquippedItem ItemBet { get; set; }
		}

		#endregion

		[SerializeField] private ARoom _room;

		private const string ON_BET_WON = "Won";
		private const string ON_BET_LOST = "Lost";
		private const string ON_NOTHING_TO_BET = "CannotBet";

		private int _numberOfBets = 0;
		private CroupierStatData _npc;
		private Inventory _inventory;
		private BetInfo _currentReward = new();

		private RoomRewardType _betType => _room.RoomData.Reward;

		#region Unity builtins

		private void Awake()
		{
			_npc = GetComponent<EntityIdentity>()?.Stats as CroupierStatData;
			_inventory = GameManager.Player.GetComponent<Inventory>();
		}

		private void Start() => ProcessBetReward();

		#endregion

		#region Interaction

		public string InteractionTitle => "Talk with the croupier";

		public void Interact(IInteractionActor actor)
		{
			if (_npc.DialogueData == null)
				return;
			actor.UnSuggestInteraction(this);
			OpenAndProcessDialogue(_npc.DialogueData);
		}

		private void OnTriggerEnter(Collider other) => other.GetComponent<IInteractionActor>()?.SuggestInteraction(this);

		private void OnTriggerExit(Collider other) => other.GetComponent<IInteractionActor>()?.UnSuggestInteraction(this);

		#endregion

		#region Dialogue events

		private void Bet()
		{
			if (!CanPlayerBet())
			{
				OnCannotBet();
				return;
			}

			bool isWin = Random.Range(0, 2) == 0;

			StartCoroutine(PrepareBet(isWin, onPreparationDone: () =>
			{
				if (isWin)
					OnBetWon();
				else
					OnBetLost();
				_numberOfBets++;
			}));
		}

		private void Refuse()
		{
			CloseDialogue();
			_room.Clear();
		}

		protected override string GetFormattedChoice(string choiceText)
		{
			switch (_betType)
			{
				case RoomRewardType.STARS:
					return choiceText.Replace("{BET}", _currentReward.MoneyBet.ToString())
									 .Replace("{REWARD}", _currentReward.MoneyReward.ToString());
				case RoomRewardType.ITEM:
					return choiceText.Replace("{BET}", $"{_currentReward.ItemBet.Details.name}<sprite=\"{_currentReward.ItemBet.Details.Graphics.name}\" index=0>")
									 .Replace("{REWARD}", _currentReward.ItemBet.Details.name.ToString());
			}
			return choiceText;
		}

		#endregion

		#region Betting

		#region RunMoney reward

		private int GetMoneyBet()
			=> _numberOfBets == 0 ? Mathf.RoundToInt(GameManager.RunMoney * (_npc.InitialBetRatio / 100f)) : _currentReward.MoneyBet * 2;

		#endregion

		#region Item reward

		private AEquippedItem GetItemBet()
		{
			AEquippedItem[] items = _inventory.Items.Where(x => x.HasUpgrade).ToArray();

			if (items.Length == 0)
				return null;
			return items.Random();
		}

		#endregion

		private bool CanPlayerBet()
		{
			switch (_betType)
			{
				case RoomRewardType.STARS:
					return _currentReward.MoneyBet >= _npc.MinimumBet && GameManager.CanRunMoneyAfford(_currentReward.MoneyBet);
				case RoomRewardType.ITEM:
					return _currentReward.ItemBet != null;
			}
			return false;
		}

		private void ProcessBetReward()
		{
			switch (_betType)
			{
				case RoomRewardType.STARS:
					_currentReward.MoneyBet = GetMoneyBet();
					break;
				case RoomRewardType.ITEM:
					_currentReward.ItemBet = GetItemBet();
					break;
			}
		}

		private IEnumerator PrepareBet(bool isWinningBet, Action onPreparationDone)
		{
			// Hide dialogue to show some feedback process
			CloseDialogue();

			yield return new WaitForSeconds(0.5f);

			// Visually remove the money to bet
			if (_betType == RoomRewardType.STARS)
				GameManager.PayWithRunMoney(_currentReward.MoneyBet);

			yield return new WaitForSeconds(0.5f);

			onPreparationDone?.Invoke();
		}

		private void OnBetWon()
		{
			switch (_betType)
			{
				case RoomRewardType.STARS:
					GameManager.RewardWithRunMoney(_currentReward.MoneyReward);
					break;
				case RoomRewardType.ITEM:
					_currentReward.ItemBet.Upgrade();
					break;
			}
			ProcessCheckpoint(ON_BET_WON);
		}

		private void OnBetLost() => ProcessCheckpoint(ON_BET_LOST);
		private void OnCannotBet() => ProcessCheckpoint(ON_NOTHING_TO_BET);

		#endregion
	}
}

﻿using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Run.Rooms.Events;
using Nawlian.Lib.Utils;
using System;
using UnityEngine;

namespace Game.Entities.AI.Dealer
{
	public class RoomTotem : Damageable
	{
		[SerializeField] private float _chargeTime;
		private bool _active;
		private DealRoom _room;
		private RoomTotemLink _link;
		private DealerAI _ai;

		public static event Action<bool> OnStateChanged;

		public bool IsActive
		{
			get => _active;
			set
			{
				_active = value;
				SetActive(value);
			}
		}

		public override bool IsDead => !IsActive;

		protected override void Awake()
		{
			_link = GetComponentInChildren<RoomTotemLink>();
		}

		private void Start()
		{
			_room = RunManager.CurrentRoomInstance as DealRoom;
		}

		private void ReActivate()
		{
			if (!_room.Cleared)
				IsActive = true;
		}

		private void SetActive(bool active)
		{
			_link.SetTarget(active ? _room.Boss.transform : null);
			OnStateChanged?.Invoke(active);
		}

		#region IDamageProcessor

		public override float ApplyDamage(EntityIdentity attacker, float amount)
		{
			if (IsDead)
				return 0;
			else if (attacker.gameObject == _room.Boss && _room.Boss.GetComponent<DealerAI>().IsDashAttack)
			{
				IsActive = false;
				Awaiter.WaitAndExecute(_chargeTime, () => ReActivate());
			}
			return 0;
		}

		public override void ApplyKnockback(EntityIdentity attacker, Vector3 force) {}
		public override void ApplyPassiveDamage(float amount) {}
		public override void Kill(EntityIdentity attacker){}

		#endregion
	}
}
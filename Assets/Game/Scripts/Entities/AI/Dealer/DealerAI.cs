﻿using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Systems.Run.Rooms.Events;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Entities.AI.Dealer
{
	public class DealerAI : EnemyAI, IAnimationEventListener
	{
		protected override bool IsBasicEnemy => false;

		[SerializeField] private Vector3 _defaultRotationNormal;
		[SerializeField] private LayerMask _wallMask;

		private DealerDialogue _dialogue;
		private DealerStatData _stats;
		private DealRoom _dealRoom;
		private bool _activated = false;
		private bool _activatedOnce = false;
		private Vector3 _gfxOffset;

		private Vector3 _mapCenter => _dealRoom.BossSpawnPoint.WithY(transform.position.y);
		private bool _atTheMapCenter => Vector3.Distance(transform.position, _mapCenter) <= 0.5f;

		#region Unity builtins

		protected override void Awake()
		{
			base.Awake();
			_dialogue = GetComponentInChildren<DealerDialogue>();
			_gfxOffset = _gfxAnim.transform.localPosition;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as DealerStatData;
			_dealRoom = _room as DealRoom;
			_attackNumber = 1; // Start with laser
			_activated = false;
			_activatedOnce = false;
		}

		#endregion

		#region Movement

		protected override bool UsesPathfinding => true;
		protected override float TriggerRange => Mathf.Infinity;
		protected override float UnTriggerRange => Mathf.Infinity;

		protected override void Update()
		{
			if (_dialogue.Apologizing)
				return;
			base.Update();

			if (!_activatedOnce) 
				return;

			if (!IsDashAttack || !_activated)
				NextAggressivePosition = _mapCenter;
			if (_activatedOnce && !_activated && _atTheMapCenter)
			{
				State = Shared.EntityState.STUN;
				_dialogue.Apologize();
			}
		}

		#endregion

		#region States

		protected override void OnInitState()
		{
			// We don't want to activate the AI here, the room should take care of it with TakeAction()
		}

		protected override Vector3 GetTargetPosition()
		{
			if (_activated)
				return base.GetTargetPosition();
			return transform.position - transform.right;
		}

		protected override void ResetStates()
		{
			base.ResetStates();
			_gfxAnim.SetBool("IsDashing", false);
			_gfxAnim.SetBool("IsCasting", false);
		}

		public void TakeAction()
		{
			_activated = true;
			_activatedOnce = true;
			_room.Activate();

			// TODO: game feel before starting the fight here

			// Set AI as active
			ResetStates();
		}

		public void Stop()
		{
			ResetStates();
			UnlockTarget();
			LockAim = false;
			_entity.SetInvulnerable(true);
			_activated = false;

			if (_laserInstance != null)
				_laserInstance.Release();
		}

		#endregion

		#region Attacks

		[SerializeField] private ModularAttack _laserInstance;
		protected override float AttackRange => 99999;
		protected override float AttackCooldown => 1.5f;

		private int _attackNumber;
		private int _dashToPerform;
		public bool IsDashAttack => _attackNumber % 2 == 0;

		protected override void TryAttacking()
		{
			if (!_activated)
				return;
			base.TryAttacking();
		}

		protected override void Attack()
		{
			if (!_activated)
				return;
			else if (IsDashAttack)
			{
				if (_dashToPerform == 0)
					_dashToPerform = Random.Range(_stats.ConsecutiveDashes.x + 1, _stats.ConsecutiveDashes.y + 2);
				AttackBase.ShowAttackPrevisu(_stats.DashAttack, transform.position, .5f, this, 
					OnUpdate: (param) 
					=> param.Transform.localScale = new Vector3(1, 1, Vector3.Distance(transform.position, transform.position + GetAimNormal() * GetDistanceToWall())));
				_gfxAnim.Play(_stats.StartDashAnimation.name);
			}
			else
			{
				LockMovement = true;
				LockAim = true;
				if (!_atTheMapCenter)
					Teleport(_mapCenter);
				_gfxAnim.SetBool("IsCasting", true);
			}
		}

		private void Teleport(Vector3 position)
		{
			ObjectPooler.Get(_stats.TeleportationFx, transform.position + Vector3.up, Quaternion.identity, null);
			transform.position = position;
			ObjectPooler.Get(_stats.TeleportationFx, transform.position + Vector3.up, Quaternion.identity, null);
			_gfxAnim.transform.localPosition = _gfxOffset;
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (!_activated)
				return;
			else if (animationArg != "Attack")
				return;
			if (IsDashAttack)
			{
				_dashToPerform--;
				if (_dashToPerform == 0)
					_attackNumber++;
				ModularAttack instance = AttackBase.Spawn(_stats.DashAttack, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _stats.DashAttack
				}).GetComponent<ModularAttack>();
				instance.OnStart(Vector3.zero, 0);
				DashToWall();
			}
			else if (_atTheMapCenter)
			{
				_laserInstance = AttackBase.Spawn(_stats.LaserAttack, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _stats.LaserAttack
				}).GetComponent<ModularAttack>();
				_laserInstance.OnStart(Vector3.zero, 0);
				Awaiter.WaitAndExecute(_stats.LaserAttack.ActiveTime, () =>
				{
					ResetStates();
					OnAttackEnd();
					_attackNumber++;
				});
			}
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName(_stats.StartDashAnimation.name))
			{
				LockTarget(GameManager.Player.transform);
				LockMovement = true;
			}
			else if (stateInfo.IsName("Dash"))
			{
				LockTarget(GameManager.Player.transform, true);
				LockAim = true;
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("Dash") && !_gfxAnim.GetBool("IsDashing") && LockMovement)
			{
				ResetStates();
				OnAttackEnd();
			}
		}

		private float GetDistanceToWall()
		{
			Ray ray = new Ray(transform.position + Vector3.up, GetAimNormal());

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _wallMask))
				return Vector3.Distance(transform.position.WithY(_room.GroundLevel), hit.point.WithY(_room.GroundLevel));
			return 0;
		}

		private void DashToWall()
		{
			const float SPEED = 50f; // 5m in 0.1s
			float distanceToWall = GetDistanceToWall();
			float dashTime = distanceToWall / SPEED;

			_gfxAnim.SetBool("IsDashing", true);
			Dash(GetAimNormal(), distanceToWall, dashTime, false, false);
			Awaiter.WaitAndExecute(dashTime, () =>
			{
				ResetStates();
				OnAttackEnd();
			});
		}

		#endregion

		protected override void OnDeath(Damageable damageable) => Stop();
	}
}

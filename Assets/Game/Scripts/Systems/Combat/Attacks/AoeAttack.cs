﻿using Game.Entities.Shared;
using Game.Systems.Combat.Effects;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	[RequireComponent(typeof(AudioSource))]
	public class AoeAttack : AttackBase
	{
		private AoeAttackData _attackData;
		public override bool FollowCaster => _attackData.FollowCaster;
		public override float Range => _attackData.Range;

		private List<Collider> _hitColliders = new List<Collider>();
		private float _startTime;

		private ParticleSystem _particleSystem;
		private AudioSource _source;
		private ParticleSystem[] _pss;
		private bool _isOff = false;

		public override void Init(object data)
		{
			base.Init(data);
			_startTime = Time.time;
			_hitColliders.Clear();
			_attackData = _data as AoeAttackData;

			var main = _particleSystem.main;

			main.duration = _attackData.ActiveTime;
			main.startLifetime = _attackData.ActiveTime;

			_particleSystem.Play(true);
			_source.PlayOneShot(_attackData.IdleAudio);
			_isOff = false;
		}

		protected override void OnReleasing()
		{
			base.OnReleasing();
			_particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}

		private void Awake()
		{
			_pss = GetComponentsInChildren<ParticleSystem>(true);
			_source = GetComponent<AudioSource>();
			_particleSystem = GetComponentInChildren<ParticleSystem>();
		}

		public override void OnStart(Vector3 offset, float travelDistance)
		{
			if (_attackData.ScaleOverLifetime)
				Tween.LocalScale(transform, _attackData.EndScale, _attackData.ActiveTime, 0, Tween.EaseLinear);
			if (_attackData.RotateOverTime)
				Tween.Rotate(transform, _attackData.EndRotation, Space.Self, _attackData.ActiveTime, 0, Tween.EaseLinear);
		}

		private void Update()
		{
			if (_isOff)
				return;
			if (Time.time - _startTime >= _attackData.ActiveTime)
				SmoothRelease();
		}

		private void SmoothRelease()
		{
			_isOff = true;
			if (_pss?.Length > 0)
			{
				float time = _pss.Max(x => x.main.startLifetime.constantMax) + _pss.Max(x => x.main.startDelay.constantMax);
				Invoke(nameof(Release), time);
			}
			else
				Release();
		}

		public override void OnAttackHit(Collider collider)
		{
			if (collider.gameObject.layer == Caster.gameObject.layer || _isOff)
				return;
			else if (Time.time - _startTime <= 0.1f && !_hitColliders.Contains(collider))
			{
				IDamageProcessor damageProcessor = collider.GetComponent<IDamageProcessor>();

				_hitColliders.Add(collider);
				if (damageProcessor != null)
				{
					Vector3 direction = _attackData.KnockbackDir == KnockbackDirection.FORWARD ? transform.forward : (collider.transform.position - transform.position).normalized.WithY(0);
					float knockbackForce = Caster.Scale(_attackData.BaseKnockbackForce, StatModifier.KnockbackForce);
					float totalDamage = Caster.Scale(_attackData.BaseDamage, StatModifier.AttackDamage);

					damageProcessor.ApplyDamage(Caster, totalDamage);
					damageProcessor.ApplyKnockback(Caster, direction * knockbackForce);
					ObjectPooler.Get(_attackData.HitFx, collider.transform.position.WithY(transform.position.y), Quaternion.Euler(0, transform.rotation.eulerAngles.y + _attackData.HitYRotation, 0), null);
				}
			}

			EffectProcessor processor = collider.GetComponent<EffectProcessor>();
			EntityIdentity entity = collider.GetComponent<EntityIdentity>();

			if (processor != null && entity != null && !entity.IsInvulnerable)
				processor.ApplyEffect(_attackData.Effect, _attackData.EffectDuration);
		}

#if UNITY_EDITOR
		public override (bool isValid, string message) IsAttackEditorValid()
		{
			var colliderCheck = GetComponents<Collider>();

			if (colliderCheck.Length == 0)
				return (false, "No collider found, this attack won't hit");
			else if (colliderCheck.Any(x => x.isTrigger == false))
				return (false, "Some colliders are not triggers, those won't hit");

			var psCheck = GetComponents<ParticleSystem>();

			if (psCheck.Length == 0)
				return (false, "No particle system found");

			return (true, "");
		}
#endif
	}
}

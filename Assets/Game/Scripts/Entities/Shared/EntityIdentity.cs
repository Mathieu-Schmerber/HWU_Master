using Game.Scriptables;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.Entities.Shared
{
	public class EntityIdentity : MonoBehaviour
	{
		[SerializeField] private BaseStatData _stats;

		private BaseStatData _cachedStat;
		private float _currentHealth;

		public float CurrentHealth { get => _currentHealth; set => _currentHealth = Mathf.Clamp(value, 0, _stats.MaxHealth.Value); }
		public BaseStatData Stats => _cachedStat;

		private void Awake()
		{
			_cachedStat = _stats.Clone() as BaseStatData; // Clone scriptable object so that we can edit it
			CurrentHealth = Stats.MaxHealth.Value;
		}
	}
}
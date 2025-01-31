﻿using Game.VFX.Preview;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Combat.Attacks
{
	public enum KnockbackDirection
	{
		FROM_CENTER,
		FORWARD
	}

	public abstract class AttackBaseData : ScriptableObject
	{
		public const string ATTACK_PREFAB_FOLDER = "Assets/Game/Resources/Prefabs/Attacks";

		[AssetsOnly, AssetSelector, ValidateInput("@Validate()", "@GetError()")]
		public AttackBase Prefab;

		[AssetsOnly, AssetSelector]
		public PreviewBase Previsualisation;

		[Title("Audio")]
		public AudioClip[] OnHitAudios;

		[Title("Attack stats")]
		public int BaseDamage;
		public int BaseKnockbackForce;
		public KnockbackDirection KnockbackDir;

		public abstract float AttackRange { get; }
		public abstract bool StickToCaster { get; }

#if UNITY_EDITOR

		private string GetError()
		{
			if (Prefab == null)
				return "Prefab is null";
			else
				return Prefab.IsAttackEditorValid().message;
		}

		public bool Validate()
		{
			if (Prefab == null || !Prefab.IsAttackEditorValid().isValid)
				return false;
			return true;
		}

#endif
	}
}

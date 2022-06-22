using Game.Entities.Shared.Health;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
	public class HighlightOnDamage : MonoBehaviour
	{
		[SerializeField] private Material _highlight;
		[SerializeField] private float _highlightTime;

		private Material[] _defaultMaterials;
		private Renderer _renderer;
		private Damageable _damageable;

		private void OnEnable()
		{
			_damageable.OnDamaged += OnDamageDealt;
		}

		private void OnDisable()
		{
			_damageable.OnDamaged -= OnDamageDealt;
		}

		private void Awake()
		{
			_renderer = GetComponentInChildren<Renderer>();
			_defaultMaterials = _renderer.materials;
			_damageable = GetComponent<Damageable>();
		}

		private IEnumerator Highlight()
		{
			Material[] currentMaterials = _renderer.materials;
			for (int i = 0; i < currentMaterials.Length; i++)
				currentMaterials[i] = _highlight;
			_renderer.materials = currentMaterials;
			yield return new WaitForSeconds(_highlightTime);
			_renderer.materials = _defaultMaterials;
		}

		public void OnDamageDealt(float damage)
		{
			if (!_damageable.IsDead)
				StartCoroutine(Highlight());
		}
	}
}
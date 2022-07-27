using Game.Managers;
using Game.Tools;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Player
{
	public class Footstep : MonoBehaviour
	{
		private ParticleSystem _ps;
		private Animator _animator;
		private RandomAudioClip _rdn;
		private PlayerController _controller;

		[SerializeField] private float _minSpeedToPlay;
		[SerializeField] private float _maxSpeedToPlay;

		private void Awake()
		{
			_ps = GetComponent<ParticleSystem>();
			_animator = GameManager.Player.GetComponentInChildren<Animator>();
			_rdn = GetComponent<RandomAudioClip>();
			_controller = GameManager.Player;
		}

		public void Play()
		{
			float speed = _animator.GetFloat("Speed");

			if (speed >= _minSpeedToPlay && speed <= _maxSpeedToPlay)
				_ps.Play(true);
			_rdn.PlayRandom();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger || !other.gameObject.isStatic || _controller.State != Shared.EntityState.IDLE)
				return;
			Play();
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Nawlian.Lib.Utils;
using Nawlian.Lib.Extensions;
using Game.Managers;
using Game.Entities.Shared;
using Game.Scriptables;

namespace Game.Entities.Player
{
	public class PlayerController : AController
	{
		#region Properties

		[SerializeField] private float _dashTime = 0.1f;
		[SerializeField] private ParticleSystem _dashFx;

		private Timer _dashTimer = new();
		private InputManager _inputs;

		public bool CanDash => CanMove && _dashTimer.IsOver();

		#endregion

		#region Unity builtins

		private void OnEnable()
		{
			_inputs.OnDashPressed += OnDashInput;
		}

		private void OnDisable()
		{
			_inputs.OnDashPressed -= OnDashInput;
		}

		protected override void Awake()
		{
			base.Awake();
			_inputs = InputManager.Instance;
			_dashTimer.Start(_entity.Scale(_entity.Stats.DashCooldown, StatModifier.DashCooldown), false);
		}

		#endregion

		private void OnDashInput()
		{
			if (CanDash)
			{
				Vector3 direction = GetMovementNormal().magnitude > 0 ? GetMovementNormal() : GetAimNormal();

				_dashFx.Play(true);
				Dash(direction, _entity.Scale(_entity.Stats.DashRange, StatModifier.DashRange), _dashTime);
				_dashTimer.Interval = _entity.Scale(_entity.Stats.DashCooldown, StatModifier.DashCooldown);
				_dashTimer.Restart();
			}
		}

		#region Abstraction

		protected override Vector3 GetMovementsInputs() => _inputs.MovementAxis.ToVector3XZ().ToIsometric();

		protected override Vector3 GetTargetPosition()
		{
			Vector3 aimInput = GetMovementsInputs();
			bool isAiming = aimInput.magnitude > 0;

			if (IsAimLocked)
				return _rb.position + _graphics.transform.forward;
			else if (isAiming)
				return _rb.position + aimInput;
			return _rb.position + _graphics.transform.forward;
		}

		#endregion
	}
}
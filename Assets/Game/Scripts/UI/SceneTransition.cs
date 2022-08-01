using Game.Managers;
using Nawlian.Lib.Utils;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SceneTransition : AMenu
    {
		private Material _material;
		private Canvas _canvas;

		public override bool RequiresGameFocus => true;

		protected override void Awake()
		{
			base.Awake();
			_material = GetComponentInChildren<Image>().material;
			_canvas = GetComponent<Canvas>();
		}

		private void Start()
		{
			_canvas.worldCamera = GameManager.ActiveCamera;
		}

		private void Fade(float duration, bool toOpen)
		{
			float init = toOpen ? 1 : 0;
			float end = toOpen ? 0 : 1;

			Tween.Value(init, end, (value) => _material.SetFloat("_Progress", value), duration, 0);
		}

		public override void Open()
		{
			_canvas.worldCamera = GameManager.ActiveCamera;
			GuiManager.CloseAll();
			GuiManager.Hide(_duration);
			Fade(_duration, true);
			base.Open();
		}

		public override void Close()
		{
			GuiManager.Show(_duration);
			GuiManager.OpenMenu<PlayerUi>();
			Fade(_duration, false);
			base.Close();
		}

#if UNITY_EDITOR

		public override void CloseEditorButton()
		{
			base.CloseEditorButton();
			GetComponentInChildren<Image>().material.SetFloat("_Progress", 1);
		}

		public override void OpenEditorButton()
		{
			base.OpenEditorButton();
			GetComponentInChildren<Image>().material.SetFloat("_Progress", 0);
		}

#endif

		public static void StartTransition(Action onReady)
		{
			var menu = GuiManager.OpenMenu<SceneTransition>();
			Awaiter.WaitAndExecute(menu._duration, onReady);
		}

		public static void EndTransition(Action onReady)
		{
			var menu = GuiManager.CloseMenu<SceneTransition>();
			Awaiter.WaitAndExecute(menu._duration, onReady);
		}
	}
}

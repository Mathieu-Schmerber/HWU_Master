using Nawlian.Lib.Systems.Saving;
using Nawlian.Lib.Utils;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Managers
{
	public class AudioManager : ManagerSingleton<AudioManager>, ISaveable
	{
		private Dictionary<AudioClip, AudioSource> _sources = new();
		private AudioSource _currentTrack = null;
		[SerializeField] private AudioClip _mainMenuMusic;
		[SerializeField] private AudioMixerGroup[] _mixers;
		[SerializeField] private AudioMixerGroup _musicMixerGroup;

		[System.Serializable]
		internal struct SaveData
		{
			public Dictionary<string, float> AudioLevels;
		}

		private void Start() => PlayTheme(_mainMenuMusic);

		public static void PlayTheme(AudioClip clip, bool restart = true)
		{
			if (!Instance._sources.ContainsKey(clip))
				Instance.AddNewThemeSource(clip);
			if (Instance._currentTrack == null)
			{
				Instance._currentTrack = Instance._sources[clip];
				Instance._currentTrack.Play();
				return;
			}

			if (Instance._sources[clip] == Instance._currentTrack)
				return;

			if (restart)
				Instance._sources[clip].time = 0;
			Instance._sources[clip].Play();
			Instance._sources[clip].volume = 0;

			Tween.Volume(Instance._currentTrack, 0, 1f, 0);
			Tween.Volume(Instance._sources[clip], 1, 1f, 0);

			Instance._currentTrack = Instance._sources[clip];
		}

		private void AddNewThemeSource(AudioClip clip)
		{
			var source = gameObject.AddComponent<AudioSource>();

			source.clip = clip;
			source.playOnAwake = false;
			source.loop = true;
			source.outputAudioMixerGroup = _musicMixerGroup;
			_sources.Add(clip, source);
		}

		public void Load(object data)
		{
			SaveData save = (SaveData)data;

			foreach (var group in _mixers)
				group.audioMixer.SetFloat(group.name, save.AudioLevels[group.name]);
		}

		public object Save() => new SaveData()
		{
			AudioLevels = _mixers.ToDictionary(x => x.name, (x) =>
			{
				float value;
				x.audioMixer.GetFloat(x.name, out value);
				return value;
			})
		};
	}
}
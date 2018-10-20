using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @zhenhaiwang
/// </summary>
namespace Framework
{
    public enum SoundType
    {
        SHOT = 0,
        BGM,
        COUNT,
    }

    public class SoundManager : MonoSingleton<SoundManager>
    {
        private sealed class SoundClip
        {
            public AudioClip audioClip { get; set; }
            public float clipVolume { get; set; }
            public Action onCompleteCallback { get; set; }
        }

        private Dictionary<SoundType, AudioSource> _audioSourceDict = new Dictionary<SoundType, AudioSource>();
        private Dictionary<SoundType, SoundClip> _soundClipDict = new Dictionary<SoundType, SoundClip>();
        private Stack<SoundClip> _loopSoundStack = new Stack<SoundClip>();

        private GameObject _root;

        private void Awake()
        {
            for (SoundType type = SoundType.SHOT; type < SoundType.COUNT; type++)
            {
                _audioSourceDict.Add(type, CreateAudioSource(type));
                _soundClipDict.Add(type, new SoundClip());
            }
        }

        private void Update()
        {
            for (SoundType type = SoundType.SHOT; type < SoundType.COUNT; type++)
            {
                AudioSource audioSource = _audioSourceDict[type];

                if (audioSource.clip != null)
                {
                    if (audioSource.volume != _soundClipDict[type].clipVolume)
                    {
                        audioSource.volume = _soundClipDict[type].clipVolume;
                    }

                    if (!audioSource.loop && audioSource.time + 0.003f >= audioSource.clip.length)
                    {
                        Stop(type);
                    }
                }
            }
        }

        private AudioSource CreateAudioSource(SoundType soundType)
        {
            if (_root == null)
            {
                _root = UnityUtils.AddChild(gameObject);
                _root.name = "AudioSources";
            }

            AudioSource audioSource = _root.AddComponent<AudioSource>();
            audioSource.loop = soundType == SoundType.BGM ? true : false;
            audioSource.playOnAwake = false;
            audioSource.volume = 1f;

            return audioSource;
        }

        private void PlaySoundClip(SoundType soundType, SoundClip soundClip)
        {
            Stop(soundType);

            if (soundClip != null)
            {
                _soundClipDict[soundType] = soundClip;

                _audioSourceDict[soundType].clip = soundClip.audioClip;
                _audioSourceDict[soundType].volume = soundClip.clipVolume;
                _audioSourceDict[soundType].Play();
            }
        }

        private SoundClip Play(SoundType soundType, string soundPath, float volume = 1f, Action onCompleteCallback = null)
        {
            SoundClip soundClip = new SoundClip();
            soundClip.audioClip = Resources.Load<AudioClip>(string.Format("Sounds/{0}", soundPath));
            soundClip.clipVolume = volume;
            soundClip.onCompleteCallback = onCompleteCallback;

            PlaySoundClip(soundType, soundClip);

            return soundClip;
        }

        public void PlayOneShot(string soundPath, float soundVolume = 1f, Action onCompleteCallback = null)
        {
            Play(SoundType.SHOT, soundPath, soundVolume, onCompleteCallback);
        }

        public void PushBGM(string soundPath, float soundVolume = 1f, Action onCompleteCallback = null)
        {
            SoundClip soundClip = Play(SoundType.BGM, soundPath, soundVolume, onCompleteCallback);
            _loopSoundStack.Push(soundClip);
        }

        public void PopBGM()
        {
            _loopSoundStack.Pop();

            if (_loopSoundStack.Count > 0)
            {
                PlaySoundClip(SoundType.BGM, _loopSoundStack.Peek());
            }
            else
            {
                Stop(SoundType.BGM);
            }
        }

        public void Pause(SoundType soundType)
        {
            _audioSourceDict[soundType].Pause();
        }

        public void Resume(SoundType soundType)
        {
            _audioSourceDict[soundType].UnPause();
        }

        public void Stop(SoundType soundType)
        {
            _audioSourceDict[soundType].Stop();
            _audioSourceDict[soundType].clip = null;
            _audioSourceDict[soundType].volume = 1f;

            _soundClipDict[soundType].onCompleteCallback.Call();
            _soundClipDict[soundType].onCompleteCallback = null;
        }

        public void SetVolume(SoundType soundType, float volume)
        {
            _soundClipDict[soundType].clipVolume = volume;
        }
    }
}
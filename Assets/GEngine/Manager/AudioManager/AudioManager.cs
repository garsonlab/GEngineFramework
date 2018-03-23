/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: AudioManager
 * Date    : 2018/03/12
 * Version : v1.0
 * Describe: 
 */

using System;
using System.Collections.Generic;
using GEngine.Patterns;
using UnityEngine;

namespace GEngine.Managers
{
    /// <summary>
    /// 声音管理
    /// </summary>
    public class AudioManager : Manager
    {
        public new const string NAME = "AudioManager";

        private AudioSource m_bgSource;
        private List<AudioSource> m_sources;
        private List<AudioClip> m_clips; 
        private Dictionary<AudioSource, AudioData> m_audios;
        private GameObject m_audioRoot;
        private float m_bgVolume;
        private float m_effectVolume;

        public float BgVolume { get { return m_bgVolume; } set { SetBgVolume(value); } }
        public float EffectVolume { get { return m_effectVolume; } set { SetEffectVolume(value); } }

        public override void OnRegister()
        {
            m_sources = new List<AudioSource>();
            m_clips = new List<AudioClip>();
            m_audios = new Dictionary<AudioSource, AudioData>();
            MonoManager mono = GameObject.FindObjectOfType<MonoManager>();
            if (mono == null)
            {
                GameObject obj = new GameObject("GManager");
                mono = obj.AddComponent<MonoManager>();
            }
            m_audioRoot = mono.gameObject;
            
            GarbageManager.Instance.AddCollector(GarbageCollect);
        }

        public void PlayAudio(AudioData data)
        {
            PlayAudio(Get2DAudioSource(), data);
        }

        public void PlayAudio(AudioSource source, AudioData data)
        {

        }

        public void PlayBGM(AudioData data)
        {
            if (m_bgSource == null)
                m_bgSource = m_audioRoot.AddComponent<AudioSource>();


        }


        private AudioSource Get2DAudioSource()
        {
            AudioSource audio = null;
            for (int i = 0; i < m_sources.Count; i++)
            {
                if (!m_sources[i].isPlaying)
                {
                    audio = m_sources[i];
                    break;
                }
            }

            if (audio == null && m_sources.Count < Config.Audio2DMaxPile)
            {
                audio = m_audioRoot.AddComponent<AudioSource>();
                m_sources.Add(audio);
            }

            if (audio == null)
            {
                //TODO 根据自定规则选择正在播放的替掉
                throw new Exception("当前2d播放规模已达设置最大，根据自定规则选择正在播放的替掉");
            }
            return audio;
        }

        private void SetBgVolume(float v)
        {
            m_bgVolume = Mathf.Clamp01(v);
            if (m_bgSource != null)
            {
                m_bgSource.volume = v;
            }
        }

        private void SetEffectVolume(float v)
        {
            m_effectVolume = Mathf.Clamp01(v);
            for (int i = 0; i < m_sources.Count; i++)
            {
                var er = m_audios.GetEnumerator();
                while (er.MoveNext())
                {
                    er.Current.Key.volume = v;
                }
            }
        }

        private void GarbageCollect()
        {
            
        }
    }
}
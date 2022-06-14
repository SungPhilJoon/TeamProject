using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityChanAdventure.FeelJoon
{
    public class AudioManager : Singleton<AudioManager>
    {
        #region Variables
        // public AudioClip[] bgmClips;
        public SFX[] playerSFXClips;
        public SFX[] enemySFXClips;
        public SFX[] uiSFXClips;

        public AudioSource bgmAudioSource;
        public AudioSource playerSFXAudioSource;
        public AudioSource enemySFXAudioSource;
        public AudioSource uiSFXAudioSource;

        #endregion Variables

        #region Unity Methods
        void Start()
        {
            bgmAudioSource = GetComponent<AudioSource>();
        }

        #endregion Unity Methods

        #region Helper Methods
        public void ChangeBGMAndPlayDelay(AudioClip bgm, float delay)
        {
            StopCoroutine(ChangeBGMWithDelay(bgm, delay));
            StartCoroutine(ChangeBGMWithDelay(bgm, delay));
        }

        /// <summary>
        /// 효과음을 변경해주는 함수 입니다.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfxClip"></param>
        public void PlaySFX(AudioSource audioSource, AudioClip sfxClip)
        {
            if (audioSource.isPlaying)
            {
                return;
            }

            audioSource.PlayOneShot(sfxClip);
        }

        /// <summary>
        /// 효과음을 변경해주는 함수 입니다.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfxClip"></param>
        public void PlaySFX(AudioSource audioSource, SFX[] sfxClips, string name)
        {
            foreach (SFX sfxClip in sfxClips)
            {
                if (sfxClip.name.GetHashCode().Equals(name.GetHashCode()))
                {
                    if (audioSource.isPlaying)
                    {
                        return;
                    }

                    audioSource.PlayOneShot(sfxClip.sfx);
                }
            }
        }

        /// <summary>
        /// 효과음을 강제로 변경해주는 함수 입니다.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfxClips"></param>
        /// <param name="name"></param>
        public void PlayForceSFX(AudioSource audioSource, AudioClip sfxClip)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(sfxClip);
        }

        /// <summary>
        /// 효과음을 강제로 변경해주는 함수 입니다.
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="sfxClips"></param>
        /// <param name="name"></param>
        public void PlayForceSFX(AudioSource audioSource, SFX[] sfxClips, string name)
        {
            foreach (SFX sfxClip in sfxClips)
            {
                if (sfxClip.name.GetHashCode().Equals(name.GetHashCode()))
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(sfxClip.sfx);
                }
            }
        }

        private IEnumerator ChangeBGMWithDelay(AudioClip bgm, float delay)
        {
            bgmAudioSource.Pause();
            bgmAudioSource.clip = bgm;

            yield return new WaitForSeconds(delay);

            bgmAudioSource.Play();
        }

        #endregion Helper Methods
    }

    [Serializable]
    public struct SFX
    {
        public string name;
        public AudioClip sfx;
    }
}

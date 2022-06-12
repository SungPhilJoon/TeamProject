using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityChanAdventure.FeelJoon;

namespace UnityChanAdventure.KyungSeo
{
    public class Settings : MonoBehaviour
    {
        #region Variables

        private MainPlayerController _playerController;
        private ClickDragCamera _camera;
        public Light light;
        
        public AudioMixer mixer;
        public Slider bgmSlider;
        public Slider sfxSlider;
        public Slider brightnessSlider;
        public Slider sensitivitySlider;
        public GameObject aboutUs;

        #endregion

        #region Properties

        // 여기에 프로퍼티를 선언합니다.

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _playerController = FindObjectOfType<MainPlayerController>();
            _camera = FindObjectOfType<ClickDragCamera>();

            bgmSlider.minValue = 0f;
            bgmSlider.maxValue = 1f;
            bgmSlider.value = 0.7f;
            mixer.SetFloat("GameVolume", Mathf.Log10(bgmSlider.value) * 20);

            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = 0.7f;
            mixer.SetFloat("SFXVolume", Mathf.Log10(sfxSlider.value) * 20);

            brightnessSlider.minValue = 0f;
            brightnessSlider.maxValue = 1f;
            brightnessSlider.value = 0.85f;

            sensitivitySlider.minValue = 0f;
            sensitivitySlider.maxValue = 1f;
            sensitivitySlider.value = 0.7f;

            AudioListener.volume = bgmSlider.value;
            light.intensity = brightnessSlider.value;
            _camera.cameraSensitivy = sensitivitySlider.value * 10;
        }

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        #endregion

        #region UI : Sliders

        public void SetLevel(float sliderValue)
        {
            if (sliderValue == 0.01)
            {
                mixer.SetFloat("GameVolume", -80f);
                bgmSlider.value = sliderValue;

                return;
            }

            mixer.SetFloat("GameVolume", Mathf.Log10(sliderValue) * 20);
            bgmSlider.value = sliderValue;
        }

        public void SetSFXLevel(float sliderValue)
        {
            if (sliderValue == 0)
            {
                mixer.SetFloat("SFXVolume", -80f);
                sfxSlider.value = sliderValue;

                return;
            }

            mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
            sfxSlider.value = sliderValue;
        }

        public void SetBrightness(float sliderValue)
        {
            brightnessSlider.value = sliderValue;
            light.intensity = sliderValue;
        }

        public void SetSensitivity(float sliderValue)
        {
            sensitivitySlider.value = sliderValue;
            _camera.cameraSensitivy = sliderValue * 10;
        }

        #endregion
        
        #region UI : Buttons

        public void OnExitClick()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            gameObject.SetActive(false);
            if (_playerController.isSettingOn)
            {
                Time.timeScale = 1;
                _playerController.isSettingOn = false;
            }
        }

        public void OnMuteButton()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            // AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
            bgmSlider.value = bgmSlider.value == 0 ? 1 : 0;
            if (bgmSlider.value.Equals(0))
            {
                mixer.SetFloat("GameVolume", -80f);
            }
            else
            {
                mixer.SetFloat("GameVolume", 0f);
            }
        }

        public void OnSFXMuteButton()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            // AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
            sfxSlider.value = sfxSlider.value == 0 ? 1 : 0;
            if (sfxSlider.value.Equals(0))
            {
                mixer.SetFloat("SFXVolume", -80f);
            }
            else
            {
                mixer.SetFloat("SFXVolume", 0f);
            }
        }

        public void OnClickAboutUs()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            aboutUs.SetActive(true);
        }

        public void OnExitTMI()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            aboutUs.SetActive(false);
        }

        #endregion
    }
}
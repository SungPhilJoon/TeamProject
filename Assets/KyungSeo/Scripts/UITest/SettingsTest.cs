using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace ETeam.KyungSeo
{
    public class SettingsTest : MonoBehaviour
    {
        #region Variables

        private TestPlayerController _playerController;
        private ClickDragCamera _camera;
        
        public AudioMixer mixer;
        public Slider audioSlider;
        public Slider sensitivitySlider;
        public GameObject aboutUs;

        #endregion

        #region Properties

        // 여기에 프로퍼티를 선언합니다.

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _playerController = FindObjectOfType<TestPlayerController>();
            _camera = FindObjectOfType<ClickDragCamera>();
        }

        #endregion
        
        #region UI : Sliders

        public void SetLevel(float sliderValue)
        {
            mixer.SetFloat("GameVolume", Mathf.Log10(sliderValue) * 20);
        }

        // 좀 더 고민이 필요...
        public void SetSensitivity(float sliderValue)
        {
            _camera.cameraSensitivy = sliderValue;
        }

        #endregion
        
        #region UI : Buttons

        public void OnExitClick()
        {
            gameObject.SetActive(false);
            if (_playerController.isSettingOn)
            {
                Time.timeScale = 1;
                _playerController.isSettingOn = false;
            }
        }

        public void OnMuteButton()
        {
            AudioListener.volume = AudioListener.volume == 0 ? 1 : 0;
        }

        public void OnClickAboutUs()
        {
            aboutUs.SetActive(true);
        }

        public void OnExitTMI()
        {
            aboutUs.SetActive(false);
        }

        #endregion
    }
}
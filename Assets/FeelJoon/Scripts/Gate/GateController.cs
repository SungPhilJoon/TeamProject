using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class GateController : MonoBehaviour
    {
        #region Event
        public event Action OnEnterGate;

        #endregion Event

        #region Variables
        [SerializeField] private Transform arrivalPoint;

        [SerializeField] private AudioClip bgmClip;

        private CharacterController controller;

        #endregion Variables

        #region Unity Methods
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                controller = other.GetComponent<CharacterController>();

                controller.enabled = false;
                LoadingUIManager.Instance.LoadScene();
                other.transform.position = arrivalPoint.position;
                controller.enabled = true;

                AudioManager.Instance.ChangeBGMAndPlayDelay(bgmClip, 3.5f);

                OnEnterGate?.Invoke();
            }
        }

        #endregion Unity Methods
    }
}
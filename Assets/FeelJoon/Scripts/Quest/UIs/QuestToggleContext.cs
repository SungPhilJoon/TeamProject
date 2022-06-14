using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class QuestToggleContext : MonoBehaviour
    {
        #region Variables
        public QuestDisplayGroup questDisplayGroup;
        public QuestObject questObject;

        #endregion Variables

        #region Unity Methods


        #endregion Unity Methods

        #region Helper Methods
        public void OnClick(bool isOn)
        {
            if (isOn)
            {
                AudioManager.Instance.PlayForceSFX(
                AudioManager.Instance.uiSFXAudioSource,
                AudioManager.Instance.uiSFXClips,
                "BtnClick");

                questDisplayGroup.gameObject.SetActive(true);

                questDisplayGroup.Show(questObject, isOn);
            }
        }

        #endregion Helper Methods
    }
}
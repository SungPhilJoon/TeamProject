using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityChanAdventure.FeelJoon;

namespace UnityChanAdventure.YongHak
{
    public class ScrollViewController : MonoBehaviour
    {
        #region Variables
        private ScrollRect scrollRect;
        public float space = 50f;
        public GameObject uiPrefab;

        public GameObject uiSword;
        public GameObject uiBow;
        public GameObject uiItem;
        public List<RectTransform> uiObjects = new List<RectTransform>();

        public Shop shop;

        #endregion Variables
        
        #region Method
        void Start()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        //스크롤 뷰에 새로은 오브젝트를 추가하는 코드
        public void AddNewUiObject()
        {
            var newUi = Instantiate(uiPrefab, scrollRect.content).GetComponent<RectTransform>();
            uiObjects.Add(newUi);
            
            float y = 0f;
            for(int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].anchoredPosition = new Vector2(0f, -y);
                y +=uiObjects[i].sizeDelta.y + space;
            }

            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
        }

        public void TapChange1()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            uiSword.SetActive(true);
            uiBow.SetActive(false);
            uiItem.SetActive(false);
            
            shop.dataBaseIndex = 0;
        }

        public void TapChange2()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            uiSword.SetActive(false);
            uiBow.SetActive(true);
            uiItem.SetActive(false);
            
            shop.dataBaseIndex = 1;
        }

        public void TapChange3()
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            uiSword.SetActive(false);
            uiBow.SetActive(false);
            uiItem.SetActive(true);
            
            shop.dataBaseIndex = 2;
        }
        #endregion Method
    }
}

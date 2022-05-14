using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETeam.YongHak
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
        #endregion Variables
        
        #region Method
        void Start()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        void Update()
        {
            
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
            uiSword.SetActive(true);
            uiBow.SetActive(false);
            uiItem.SetActive(false);
        }

        public void TapChange2()
        {
            uiSword.SetActive(false);
            uiBow.SetActive(true);
            uiItem.SetActive(false);
        }

        public void TapChange3()
        {
            uiSword.SetActive(false);
            uiBow.SetActive(false);
            uiItem.SetActive(true);
        }
        #endregion Method
    }
}

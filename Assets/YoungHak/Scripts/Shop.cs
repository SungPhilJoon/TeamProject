using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.YongHak;

namespace ETeam.YongHak
{
    public class Shop : MonoBehaviour
    {
        #region Variables
        public RectTransform uiGroup;
        ShopTestPlayer enterPlayer;
        public int[] itemPrice;
        public Text talkText;
        public Text coinText;
        public int coin = 10000;
        private string coinString;
        
        #endregion Variables

        #region Method

        void Start()
        {
            uiGroup.anchoredPosition = Vector3.zero;
        }

        void Update()
        {
            coinString = coin.ToString();
            coinText.text = coinString;
        }

        public void Enter(ShopTestPlayer player)
        {
            enterPlayer = player;
            uiGroup.anchoredPosition = Vector3.zero;
        }

        public void Exit()
        {
            uiGroup.anchoredPosition = Vector3.down * 2000;
        }

        public void Buy(int index)
        {
            int price = itemPrice[index];
            if(price > coin)
            {
                StopCoroutine(Talk());
                StartCoroutine(Talk());
                return;
            }

            coin -= price;
        }

        IEnumerator Talk()
        {
            talkText.text = "어이 돈이 모자라";
            yield return new WaitForSeconds(2f);
            talkText.text = "원하는게 있나?";
        }
        #endregion Method
    }
}

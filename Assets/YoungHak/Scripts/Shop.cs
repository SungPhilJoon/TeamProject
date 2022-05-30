using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETeam.YongHak;
using ETeam.FeelJoon;
using ETeam.KyungSeo;

namespace ETeam.YongHak
{
    public class Shop : MonoBehaviour, IInteractable
    {
        #region Variables
        public float Distance => distance;
        public Animator anim;
        private float distance = 3.0f;
        private new GameObject gameObject;
        [SerializeField] private ItemObjectDatabase[] database;
        public ItemObject itemObject;
        public RectTransform uiGroup;
        ShopTestPlayer enterPlayer;
        public int[] itemPrice;
        public Text talkText;
        public Text coinText;
        public int coin = 10000;
        private string coinString;
        public int dataBaseIndex;
        JsonTest jsonTest;
        
        #endregion Variables

        #region Method

        void Start()
        {
            //uiGroup.anchoredPosition = Vector3.zero;
            jsonTest = new JsonTest();
            jsonTest.Load();
        }

        void Update()
        {
            coinString = coin.ToString();
            coinText.text = coinString;
        }

        /*public void Enter(ShopTestPlayer player)
        {
            enterPlayer = player;
            uiGroup.anchoredPosition = Vector3.zero;
        }*/

        public void Exit()
        {
            uiGroup.anchoredPosition = Vector3.down * 2000;
            anim.SetTrigger("doHello");
        }

        public int getCoin()
        {
            return coin;
        }

        IEnumerator Talk()
        {
            talkText.text = "어이 돈이 모자라";
            yield return new WaitForSeconds(2f);
            talkText.text = "원하는게 있나?";
        }

        public bool Interact(GameObject other)
        {
            Debug.Log("체크1");
            float calcDistance = Vector3.Distance(transform.position, other.transform.position);
            if (calcDistance > distance)
            {
                Debug.Log(calcDistance);
                Debug.Log(distance);
                return false;
            }
            gameObject = other;
            
            return other.GetComponent<MainPlayerController>()?.Enter(uiGroup) ?? false;
        }

        public void Buy(int itemIndex)
        {
            ItemObject dropItemObject = database[dataBaseIndex].itemObjects[itemIndex];

            int price = itemPrice[dataBaseIndex];
            if(price > coin)
            {
                StopCoroutine(Talk());
                StartCoroutine(Talk());
                return;
            }
            itemObject = dropItemObject;
            gameObject.GetComponent<MainPlayerController>().inventory.AddItem(new Item(itemObject), 1);

            coin -= price;
        }

        public void StopInteract(GameObject other)
        {

        }

        #endregion Method
    }
}

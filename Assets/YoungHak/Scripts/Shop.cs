using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityChanAdventure.YongHak;
using UnityChanAdventure.FeelJoon;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.YongHak
{
    public class Shop : MonoBehaviour, IInteractable
    {
        #region Variables
        public float Distance => distance;
        public Animator anim;
        private float distance = 3.0f;
        
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

        private readonly int hashStopInteract = Animator.StringToHash("doHello");
        
        #endregion Variables

        #region Method
        void Start()
        {
            //uiGroup.anchoredPosition = Vector3.zero;
            jsonTest = new JsonTest();
            jsonTest.Load();
        }

        /*public void Enter(ShopTestPlayer player)
        {
            enterPlayer = player;
            uiGroup.anchoredPosition = Vector3.zero;
        }*/

        private void Init()
        {

        }

        public void Exit()
        {
            uiGroup.anchoredPosition = Vector3.down * 2000;
            anim.SetTrigger(hashStopInteract);
        }

        public int getCoin()
        {
            return coin;
        }

        IEnumerator Talk()
        {
            yield return new WaitForSeconds(2f);

            talkText.text = ShopNPCDialogueList.EnterDialogue;
        }

        public bool Interact(GameObject other)
        {
            float calcDistance = Vector3.Distance(transform.position, other.transform.position);
            if (calcDistance > distance)
            {
                return false;
            }
            
            return other.GetComponent<MainPlayerController>()?.Enter(uiGroup) ?? false;
        }

        public void Buy(int itemIndex)
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            ItemObject dropItemObject = database[dataBaseIndex].itemObjects[itemIndex];

            int price = dropItemObject.price; // itemPrice[dataBaseIndex];
            if(price > GameManager.Instance.Main.gold)
            {
                talkText.text = ShopNPCDialogueList.InsufficientDialogue;

                StopCoroutine(Talk());
                StartCoroutine(Talk());

                return;
            }
            else if (GameManager.Instance.Main.inventory.EmptySlotCount.Equals(0))
            {
                talkText.text = ShopNPCDialogueList.InventoryFullDialogue;

                StopCoroutine(Talk());
                StartCoroutine(Talk());

                return;
            }

            talkText.text = ShopNPCDialogueList.PurchaseDialogue;

            // itemObject = dropItemObject;
            GameManager.Instance.Main.inventory.AddItem(new Item(dropItemObject), 1);

            GameManager.Instance.Main.gold -= price;
        }

        public void StopInteract(GameObject other)
        {
            AudioManager.Instance.PlayForceSFX(
            AudioManager.Instance.uiSFXAudioSource,
            AudioManager.Instance.uiSFXClips,
            "BtnClick");

            uiGroup.anchoredPosition = Vector3.down * 2000;
            anim.SetTrigger(hashStopInteract);
        }

        #endregion Method
    }
}

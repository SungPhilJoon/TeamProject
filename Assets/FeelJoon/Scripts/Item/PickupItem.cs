using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class PickupItem : MonoBehaviour, IInteractable
    {
        #region Variables
        public float distance = 3.0f;
        public float Distance => distance;

        public ItemObject itemObject;

        #endregion Variables

        #region Unity Methods
        void OnValidate()
        {
//#if UNITY_EDITOR
//            GetComponent<SpriteRenderer>().sprite = itemObject.icon;
//#endif
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, distance);
        }

        #endregion Unity Methods

        #region IInteractable
        public bool Interact(GameObject other)
        {
            float calcDistance = Vector3.Distance(transform.position, other.transform.position);
            if (calcDistance > distance)
            {
                return false;
            }
            
            return other.GetComponent<MainPlayerController>()?.PickupItem(this) ?? false;
        }

        public void StopInteract(GameObject other)
        {

        }

        #endregion IInteractable
    }
}
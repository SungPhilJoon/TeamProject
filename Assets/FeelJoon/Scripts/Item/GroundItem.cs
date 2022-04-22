using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class GroundItem : MonoBehaviour
    {
        public ItemObject itemObject;

        void OnValidate()
        {
#if UNITY_EDITOR
            GetComponent<SpriteRenderer>().sprite = itemObject.icon;
#endif
        }
    }
}
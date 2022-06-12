using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public enum ItemType : int
    {
        //Helmet = 0,
        //Chest = 1,
        //Pants = 2,
        //Boots = 3,
        //Pauldrons = 4,
        //Gloves = 5,
        Sword = 0,
        Bow = 1,
        Food = 2,
        //Default,
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/New Item")]
    public class ItemObject : ScriptableObject
    {
        public ItemType type;
        public bool stackable;

        public Sprite icon;
        public GameObject modelPrefab;

        public int price;

        public Item data = new Item();

        public List<string> boneNames = new List<string>();

        [TextArea(15, 20)]
        public string description;

        void OnValidate()
        {
            boneNames.Clear();

            if (modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
            {
                return;
            }

            SkinnedMeshRenderer renderer = modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            Transform[] bones = renderer.bones;

            foreach (Transform t in bones)
            {
                boneNames.Add(t.name);
            }
        }

        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }
    }
}
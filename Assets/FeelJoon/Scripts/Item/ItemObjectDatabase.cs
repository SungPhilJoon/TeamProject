using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [CreateAssetMenu(fileName = "New Item Dataabase", menuName = "Inventory System/Items/Database")]
    public class ItemObjectDatabase : ScriptableObject
    {
        public ItemType databaseType;

        public ItemObject[] itemObjects;

        public void OnValidate()
        {
            switch (databaseType)
            {
                case ItemType.Sword:
                    for (int i = 0; i < itemObjects.Length; i++)
                    {
                        itemObjects[i].data.id = i + itemCodeList.swordCode;
                    }
                    break;
                case ItemType.Bow:
                    for (int i = 0; i < itemObjects.Length; i++)
                    {
                        itemObjects[i].data.id = i + itemCodeList.bowCode;
                    }
                    break;
                case ItemType.Food:
                    for (int i = 0; i < itemObjects.Length; i++)
                    {
                        itemObjects[i].data.id = i + itemCodeList.foodCode;
                    }
                    break;
            }
        }
    }

    public struct itemCodeList
    {
        public static int swordCode = 0;
        public static int bowCode = 100;
        public static int foodCode = 200;
    }
}
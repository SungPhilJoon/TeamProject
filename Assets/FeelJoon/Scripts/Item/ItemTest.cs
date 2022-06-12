using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class ItemTest : MonoBehaviour
    {
        #region Variables
        public InventoryObject inventoryObject;
        public ItemObjectDatabase[] databaseObjects;

        #endregion Variables

        #region Helper Methods
        public void AddNewItem()
        {
            int rndItemDatabaseNumber = Random.Range(0, databaseObjects.Length);

            ItemObject newItemObject = databaseObjects[rndItemDatabaseNumber].itemObjects[Random.Range(0, databaseObjects[rndItemDatabaseNumber].itemObjects.Length)];
            Item newItem = new Item(newItemObject);
            
            inventoryObject.AddItem(newItem, 1);
        }

        public void ClearInventory()
        {
            inventoryObject.Clear();
        }

        #endregion Helper Methods
    }
}
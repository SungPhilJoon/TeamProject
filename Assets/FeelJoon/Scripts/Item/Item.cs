using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [Serializable]
    public class Item
    {
        public int id = -1;
        public string name;
        public ItemType itemType;

        public ItemBuff[] buffs;

        public Item()
        {
            id = -1;
            name = "";
        }

        public Item(ItemObject itemObject)
        {
            name = itemObject.name;
            id = itemObject.data.id;
            itemType = itemObject.type;

            buffs = new ItemBuff[itemObject.data.buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(itemObject.data.buffs[i].Min, itemObject.data.buffs[i].Max)
                {
                    stat = itemObject.data.buffs[i].stat
                };
            }
        }
    }
}
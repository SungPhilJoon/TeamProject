using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public enum CharacterAttribute
    {
        Strength,
        Defensive,
        CriticalRate,
        Health,
        Mana
    }

    [Serializable]
    public class ItemBuff : IModifier
    {
        public CharacterAttribute stat;
        public int value;

        [SerializeField] private int min;
        [SerializeField] private int max;

        public int Min => min;
        public int Max => max;

        public ItemBuff(int min, int max)
        {
            this.min = min;
            this.max = max;

            GenerateValue();
        }

        public void GenerateValue()
        {
            value = UnityEngine.Random.Range(min, max);
        }

        #region IModifier interface
        public void AddValue(ref int v)
        {
            v += value;
        }

        #endregion IModifier interface
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    [Serializable]
    public class ModifiableInt
    {
        #region Events
        private event Action<ModifiableInt> OnModifiedValue;

        #endregion Events

        #region Variables
        [NonSerialized] private int baseValue;

        [SerializeField] private int modifiedValue;

        private List<IModifier> modifiers = new List<IModifier>();

        #endregion Variables

        #region Properties
        public int BaseValue
        {
            get => baseValue;

            set
            {
                baseValue = value;
                UpdateModifiedValue();
            }
        }

        public int ModifiedValue
        {
            get => modifiedValue;
            set => modifiedValue = value;
        }

        #endregion Properties

        public ModifiableInt(Action<ModifiableInt> method = null)
        {
            ModifiedValue = baseValue;
            RegisterModEvent(method);
        }

        #region Helper Methods
        public void RegisterModEvent(Action<ModifiableInt> method)
        {
            if (method != null)
            {
                OnModifiedValue += method;
            }
        }

        public void UnRegisterModEvent(Action<ModifiableInt> method)
        {
            if (method != null)
            {
                OnModifiedValue -= method;
            }
        }

        private void UpdateModifiedValue()
        {
            int valueToAdd = 0;
            foreach (IModifier modifier in modifiers)
            {
                modifier.AddValue(ref valueToAdd);
            }

            ModifiedValue = baseValue + valueToAdd;

            OnModifiedValue?.Invoke(this);
        }

        public void AddModifier(IModifier modifier)
        {
            modifiers.Add(modifier);

            UpdateModifiedValue();
        }

        public void RemoveModifier(IModifier modifier)
        {
            modifiers.Remove(modifier);

            UpdateModifiedValue();
        }

        #endregion Helper Methods
    }
}
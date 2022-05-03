using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public interface IModifier
    {
        void AddValue(ref int baseValue);
    }
}
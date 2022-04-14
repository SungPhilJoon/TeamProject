using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public abstract class ManualCollision : MonoBehaviour
    {
        #region Variables
        [SerializeField] protected Transform parent;

        #endregion Variables

        #region Helper Methods
        protected abstract void CheckCollision();

        #endregion Helper Methods
    }
}

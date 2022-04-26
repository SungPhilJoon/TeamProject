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

        public Collider[] targetColliders = new Collider[10];

        #endregion Variables

        #region Helper Methods
        public abstract void CheckCollision();

        #endregion Helper Methods
    }
}

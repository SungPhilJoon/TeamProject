using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public class FieldOfView : MonoBehaviour
    {
        #region Variables
        public float viewRadius = 5f;
    
        public LayerMask targetMask;
        public LayerMask obstacleMask;
    
        public Transform target;
    
        public float delay = 0.2f;

        private bool isTakeDamage = false;

        private bool isCalledStartMethod = false;

        #endregion Variables

        #region Unity Methods
        void OnEnable()
        {
            if (isCalledStartMethod)
            {
                StartCoroutine(FindTargetsWithDelay(delay));
            }
        }

        void Start()
        {
            isCalledStartMethod = true;
            StartCoroutine(FindTargetsWithDelay(delay));
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while(true)
            {
                yield return new WaitForSeconds(delay);
                if (isTakeDamage && Vector3.Distance(target.position, transform.position) > viewRadius)
                {
                    continue;
                }

                FindVisibleTarget();
            }
        }
    
        private void FindVisibleTarget()
        {
            this.target = null;
            isTakeDamage = false;
    
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            if (targetsInViewRadius.Length == 0)
            {
                return;
            }
            Transform target = targetsInViewRadius[0].transform;

            Vector3 dirToTarget = (target.position- transform.position);
            float dstToTarget = Vector3.Distance(transform.position, target.position);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                this.target = target;
            }
        }

        public void FindTakeDamagedTarget(Transform target)
        {
            isTakeDamage = true;
            this.target = target;
        }
    
        #endregion Helper Methods
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.KyungSeo
{
    public class TestArrow : MonoBehaviour
    {
        public Vector3 testTarget;
        public float arrowSpeed;

        private void Update()
        {
            float step = arrowSpeed * Time.deltaTime;
            if (testTarget != null)
            {
                transform.LookAt(testTarget);
                transform.position = Vector3.MoveTowards(transform.position, testTarget, step);
            }
        }

        public void SetTarget(Vector3 target)
        {
            testTarget = target;
        }
    }
}
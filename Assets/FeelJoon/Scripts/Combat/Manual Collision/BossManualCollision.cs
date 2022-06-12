using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class BossManualCollision : ManualCollision
    {
        #region Variables
        private BossController bossController;

        #endregion Variables

        #region Unity Methods
        void Awake()
        {
            bossController = GetComponentInParent<BossController>();
        }

        #endregion Unity Methods

        #region Helper Methods
        public override void CheckCollision()
        {
            StopCoroutine(CameraVibrate());
            StartCoroutine(CameraVibrate());
            targetColliders = Physics.OverlapBox(transform.position, boxSize * 0.5f, transform.rotation, bossController.targetMask);
        }

        private IEnumerator CameraVibrate()
        {
            float normalTime = 0f;

            while (normalTime <= 1f)
            {
                normalTime += Time.deltaTime;
                GameManager.Instance.CameraVibrateEffect(0.5f);

                yield return null;
            }
        }

        #endregion Helper Methods
    }
}
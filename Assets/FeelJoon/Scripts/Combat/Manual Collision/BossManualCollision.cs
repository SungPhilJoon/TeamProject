using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
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
                GameManager.Instance.mainCamera.transform.localPosition = new Vector3(Random.insideUnitCircle.x * 0.5f,
                Random.insideUnitCircle.y * 0.5f,
                GameManager.Instance.mainCamera.transform.localPosition.z);

                yield return null;
            }
        }

        #endregion Helper Methods
    }
}
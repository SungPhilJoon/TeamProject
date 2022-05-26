using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class Skill1_ShockWave : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed;
        [HideInInspector] public MainPlayerController owner;
        [HideInInspector] public int damage;

        public float delay = 1f;

        #endregion Variables

        #region Unity Methods
        void Start()
        {
            Destroy(this.gameObject, 2f);
        }

        void Update()
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    Debug.Log(owner);
                    damageable.TakeDamage(damage, owner.transform);
                }
            }
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator SetBackShockWave(float delay)
        {
            yield return new WaitForSeconds(delay);

            Destroy(this.gameObject);
        }

        #endregion Helper Methods
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        public float moveSpeed = 0f;
        public int damage;
        public float delay = 2f;

        public GameObject owner;

        #endregion Variables

        #region Unity Methods
        void OnEnable()
        {
            StartCoroutine(SetBackArrow(delay));
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                    gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator SetBackArrow(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        #endregion Helper Methods
    }
}
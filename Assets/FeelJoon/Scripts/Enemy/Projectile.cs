using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        public float moveSpeed = 0f;
        public float delay = 2f;
        private int damage;

        public EnemyController owner;

        #endregion Variables

        #region Properties
        public Transform Target
        {
            get => owner.Target;
        }

        #endregion Properties

        #region Unity Methods
        void Awake()
        {
            damage = owner.damage;
        }

        void OnEnable()
        {
            StartCoroutine(SetBackProjectile(delay));
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
            if (!gameObject.activeSelf)
            {
                return;
            }

            transform.localPosition += new Vector3(0f, 0f, moveSpeed * Time.deltaTime);
        }

        #endregion Unity Methods

        #region Helper Methods
        private IEnumerator SetBackProjectile(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        #endregion Helper Methods
    }
}
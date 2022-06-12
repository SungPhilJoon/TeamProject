using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class Arrow : MonoBehaviour
    {
        [HideInInspector] public float moveSpeed = 0f;
        [HideInInspector] public MainPlayerController owner;
        [HideInInspector] public int damage;
        public float delay = 2f;

        void OnEnable()
        {
            StartCoroutine(SetBackArrow(delay));
        }

        void Update()
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

                damageable?.TakeDamage(damage, owner.transform);
                gameObject.SetActive(false);
            }
        }

        private IEnumerator SetBackArrow(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
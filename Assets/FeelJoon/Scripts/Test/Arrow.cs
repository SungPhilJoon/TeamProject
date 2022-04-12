using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETeam.KyungSeo;

namespace ETeam.FeelJoon
{
    public class Arrow : MonoBehaviour
    {
        public float moveSpeed = 0f;
        public int damage;

        void Update()
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

            damageable?.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
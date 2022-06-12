using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityChanAdventure.KyungSeo;

namespace UnityChanAdventure.FeelJoon
{
    public class Skill1_ShockWave : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float moveSpeed;
        [SerializeField] private int increaseSwordSkill1_Damage = 20;
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
                    int finalDamage = owner.Damage + increaseSwordSkill1_Damage;
                    damageable.TakeDamage(finalDamage, owner.transform);
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
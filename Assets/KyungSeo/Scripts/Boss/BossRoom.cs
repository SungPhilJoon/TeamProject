using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.KyungSeo
{
    public class BossRoom : MonoBehaviour
    {
        [SerializeField] private BossController bossEnemy;
        public GameObject bossHPBar;

        private void Awake()
        {
            bossEnemy = FindObjectOfType<BossController>();
        }

        private void OnEnable()
        {
            bossHPBar.SetActive(false);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
                bossHPBar.SetActive(true);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                bossHPBar.SetActive(false);
        }
    }
}
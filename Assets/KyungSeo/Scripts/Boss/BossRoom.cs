using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.KyungSeo
{
    public class BossRoom : MonoBehaviour
    {
        [SerializeField] private TestBoss bossEnemy;

        private void Awake()
        {
            bossEnemy = FindObjectOfType<TestBoss>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                bossEnemy.target = other.gameObject;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                bossEnemy.target = null;
        }
    }
}
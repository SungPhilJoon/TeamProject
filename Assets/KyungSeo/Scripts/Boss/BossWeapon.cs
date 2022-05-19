using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.KyungSeo
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class BossWeapon : MonoBehaviour
    {
        private TestBoss boss;

        private void Start()
        {
            boss = FindObjectOfType<TestBoss>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerTest player = other.GetComponent<PlayerTest>();
                player.TakeDamage(boss.attackPower);
            }
        }
    }
}
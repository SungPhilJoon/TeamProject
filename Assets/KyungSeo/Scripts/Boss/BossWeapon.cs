using System;
using System.Collections;
using System.Collections.Generic;
using UnityChanAdventure.FeelJoon;
using UnityEngine;

namespace UnityChanAdventure.KyungSeo
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
                PlayerController player = other.GetComponent<PlayerController>();
                player.TakeDamage(boss.attackPower);
            }
        }
    }
}
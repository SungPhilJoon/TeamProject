using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using ETeam.KyungSeo;
using ETeam.FeelJoon;
using UnityEngine;

public class WeaponThrow : MonoBehaviour
{
    public int attackPower = 5;
    public float moveSpeed = 5.0f;
    public float disappearTime = 5.0f;

    public BossController owner;
    private Transform target;
    private Vector3 targetDir;

    private void Awake()
    {
        Destroy(gameObject, disappearTime);
        target = FindObjectOfType<MainPlayerController>().transform;
    }

    private void OnEnable()
    {
        targetDir = (target.transform.position - owner.transform.position);
        transform.LookAt(target);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            player?.TakeDamage(attackPower);
        }
    }
}

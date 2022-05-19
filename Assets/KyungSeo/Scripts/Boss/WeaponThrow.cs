using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using ETeam.KyungSeo;
using UnityEngine;

public class WeaponThrow : MonoBehaviour
{
    public int attackPower = 5;
    public float moveSpeed = 5.0f;
    public float disappearTime = 5.0f;

    private Transform target;
    private Vector3 targetDir;

    private void Awake()
    {
        Destroy(gameObject, disappearTime);
        target = FindObjectOfType<MainPlayerController>().transform;
        targetDir = (transform.position - target.transform.position);
    }

    private void Update()
    {
        transform.Translate(targetDir * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            MainPlayerController player = other.GetComponent<MainPlayerController>();
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            player.TakeDamage(attackPower);
            playerRigidbody.AddForce(new Vector3(attackPower, attackPower, attackPower), ForceMode.Impulse);
        }
    }
}

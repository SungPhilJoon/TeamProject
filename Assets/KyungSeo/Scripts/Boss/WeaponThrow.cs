using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using ETeam.KyungSeo;
using ETeam.FeelJoon;
using UnityEngine;
using Random = UnityEngine.Random;

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
            StopCoroutine(CameraVibrate());
            StartCoroutine(CameraVibrate());

            IDamageable player = other.GetComponent<IDamageable>();
            player?.TakeDamage(attackPower);
        }
    }

    #region Helper Methods
    private IEnumerator CameraVibrate()
    {
        float normalTime = 0f;

        while (normalTime <= 1f)
        {
            normalTime += Time.deltaTime;
            GameManager.Instance.mainCamera.transform.localPosition = new Vector3(Random.insideUnitCircle.x * 0.5f,
            Random.insideUnitCircle.y * 0.5f,
            GameManager.Instance.mainCamera.transform.localPosition.z);

            yield return null;
        }
    }

    #endregion Helper Methods
}

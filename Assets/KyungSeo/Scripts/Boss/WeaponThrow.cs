using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityChanAdventure.KyungSeo;
using UnityChanAdventure.FeelJoon;
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
            AudioManager.Instance.PlaySFX(
            AudioManager.Instance.enemySFXAudioSource,
            AudioManager.Instance.enemySFXClips,
            "BossAttackEffect");

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
            GameManager.Instance.CameraVibrateEffect(0.5f);

            yield return null;
        }
    }

    #endregion Helper Methods
}

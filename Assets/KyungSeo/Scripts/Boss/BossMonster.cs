using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.KyungSeo
{
    public class BossMonster : MonoBehaviour
    {
        private Animator _animator;
        public Transform target;
        public float moveSpeed;
        private bool enableAct;
        private int atkStep;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            enableAct = true;
        }

        private void RotateGolem()
        {
            Vector3 dir = target.position - transform.position;
            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);
        }

        private void MoveGolem()
        {
            if ((target.position - transform.position).sqrMagnitude >= 10)
            {
                _animator.SetBool("Walk", true);
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
            }

            if ((target.position - transform.position).sqrMagnitude < 10)
            {
                _animator.SetBool("Walk", false);
            }
        }

        private void Update()
        {
            if (enableAct)
            {
                RotateGolem();
                MoveGolem();
            }
        }

        private void GolemAtk()
        {
            if ((target.position - transform.position).sqrMagnitude < 10)
            {
                switch (atkStep)
                {
                    case 0:
                        atkStep++;
                        _animator.Play($"GolemAtk1");
                        break;
                    case 1:
                        atkStep++;
                        _animator.Play($"GolemAtk2");
                        break;
                    case 2:
                        atkStep = 0;
                        _animator.Play($"GolemAtk3");
                        break;
                }
            }
        }

        private void FreezeGolem()
        {
            enableAct = false;
        }

        private void UnFreezeGolem()
        {
            enableAct = true;
        }
    }
}
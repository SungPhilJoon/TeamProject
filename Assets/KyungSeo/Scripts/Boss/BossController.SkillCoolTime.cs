using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityChanAdventure.FeelJoon;
using System;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController), typeof(NavMeshAgent))]
public partial class BossController : MonoBehaviour, IAttackable, IDamageable
{
    #region Variables
    public bool isBossMeleeAttack_Available = true;
    public bool isBossThrowAttack_Available = true;
    //public bool isBossThrowAttack2_Available = true;

    #endregion Variables

    #region Cool Time Methods
    public IEnumerator BossMeleeAttackCoolTime()
    {
        isBossMeleeAttack_Available = false;

        yield return StartCoroutine(skillCoolTimeHandlers[BossSkillNameList.BossMeleeAttack1_Name.GetHashCode()](meleeAttackCoolDown));

        isBossMeleeAttack_Available = true;
    }

    public IEnumerator BossThrowAttackCoolTime()
    {
        isBossThrowAttack_Available = false;

        yield return StartCoroutine(skillCoolTimeHandlers[BossSkillNameList.BossThrowAttack1_Name.GetHashCode()](throwAttackCoolDown));

        isBossThrowAttack_Available = true;
    }
    // 얘도 혹시나 해서 남겨두는 거니까 최종빌드때 필요없으면 지워주세요
    // public IEnumerator BossThrowAttack2CoolTime()
    // {
    //     isBossThrowAttack2_Available = false;
    //
    //     yield return StartCoroutine(skillCoolTimeHandlers[BossSkillNameList.BossThrowAttack2_Name.GetHashCode()](jumpSitAttackCoolDown));
    //
    //     isBossThrowAttack2_Available = true;
    // }

    #endregion Cool Time Methods
}

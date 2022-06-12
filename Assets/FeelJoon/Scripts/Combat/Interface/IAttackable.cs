using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public interface IAttackable
    {
        AttackBehaviour CurrentAttackBehaviour
        {
            get;
        }

        void OnExecuteMeleeAttack();

        void OnExecuteProjectileAttack();
    }
}

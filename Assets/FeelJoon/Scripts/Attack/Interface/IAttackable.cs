using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETeam.FeelJoon
{
    public interface IAttackable
    {
        AttackBehaviour CurrentAttackBehaviour
        {
            get;
        }

        void OnExecuteAttack();
    }
}

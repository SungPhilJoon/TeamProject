using UnityEngine;

namespace ETeam.FeelJoon
{
    public interface IDamageable
    {
        bool IsAlive
        {
            get;
        }
    
        void TakeDamage(int damage, GameObject hitEffectPrefab);
    }
}

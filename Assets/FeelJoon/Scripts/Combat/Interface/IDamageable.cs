using UnityEngine;

namespace UnityChanAdventure.FeelJoon
{
    public interface IDamageable
    {
        bool IsAlive
        {
            get;
        }
    
        void TakeDamage(int damage, Transform target = null, GameObject hitEffectPrefab = null);
    }
}

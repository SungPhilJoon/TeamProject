using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossThrow : MonoBehaviour
{
    public GameObject weaponProjectile;
    public GameObject bossWeapon;
    
    public void OnThrow()
    {
        GameObject obj = Instantiate(weaponProjectile, bossWeapon.transform.position, bossWeapon.transform.rotation);
        obj.transform.parent = null;
    }
}

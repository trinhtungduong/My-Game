using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGunWeapon : Weapon
{
    public override void UpdateFire()
    {
        base.UpdateFire();
        if (isFiring)
        {
            shotDuration -= Time.deltaTime;
            if (shotDuration <= 0)
            {
                isBulletOut = true;
                shotDuration = baseShotDuration;
                muzzle.StartFire();
            }
        }
    }
}

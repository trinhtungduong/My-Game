using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGunWeapon : Weapon
{
    [SerializeField] HitEffect hitEffect;
    public override void UpdateFire()
    {
        if (bulletAmount <= 0) return;

        if (Physics.Raycast(ray, out hitInfo, ~LayerMask.NameToLayer("PickRay")))
        {
            hitEffect.SetupHit(hitInfo.point, hitInfo.normal);
            if (isBulletOut)
            {
                bulletAmount -= 1;
                hitInfo.collider.GetComponentInParent<IDamageMonster>()?.TakeDamage(damage);
                isBulletOut = false;
            }
        }

        if(shotDuration > 0f)
            shotDuration -= Time.deltaTime;

        if (isFiring)
        {           
            if (shotDuration <= 0)
            {
                isBulletOut = true;
                shotDuration = baseShotDuration;
                muzzle.StartFire();
            }
        }
    }
    public override void StopFire()
    {
        base.StopFire();
        muzzle.StopFire();
    }
}

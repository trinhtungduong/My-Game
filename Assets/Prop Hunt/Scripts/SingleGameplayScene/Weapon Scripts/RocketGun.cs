using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGun : Weapon
{
    public RocketBullet rocketBullet;
    public List<RocketBullet> listBullets;
    public override void UpdateFire()
    {
        if (bulletAmount <= 0) return;

        if (Physics.Raycast(ray, out hitInfo, ~LayerMask.NameToLayer("PickRay")))
        {
            if (isBulletOut)
            {
                bulletAmount -= 1;
                SpawnBullet((hitInfo.point - muzzle.transform.position).normalized);
                isBulletOut = false;
            }
        }

        if (shotDuration > 0f)
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
    public void SpawnBullet(Vector3 dir)
    {
        foreach(var bl in listBullets)
        {
            if (!bl.bullet.activeInHierarchy)
            {
                bl.transform.position = muzzle.transform.position;
                bl.transform.rotation = muzzle.transform.rotation;                
                bl.bullet.SetActive(true);
                bl.Shot(dir, this);
                return;
            }
        }

        var newBL = Instantiate(rocketBullet, muzzle.transform.position, muzzle.transform.rotation);
        newBL.bullet.SetActive(true);
        newBL.Shot(dir, this);
        listBullets.Add(newBL);
    }
}

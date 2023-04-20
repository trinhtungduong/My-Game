using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isFiring;
    public Muzzle muzzle;
    public float shotDuration;
    private void Update()
    {
        if(isFiring)
        {
            shotDuration -= Time.deltaTime;
            if(shotDuration <= 0)
            {
                shotDuration = 0.2f;
                muzzle.StartFire();
            }
        }
    }
    public void SetupWeapon()
    {
        isFiring = false;
    }
    public void StartFire()
    {
        isFiring = true;        
    }

    public void StopFire()
    {
        isFiring = false;
    }
}

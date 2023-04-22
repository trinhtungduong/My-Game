using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Weapon : MonoBehaviour
{
    public bool weaponSetuped;
    public bool isFiring;
    public Muzzle muzzle;
    public float shotDuration;
    public Rig rigLayer_WeaponAiming;
    public float aimDuration;
    private void Update()
    {
        if (!weaponSetuped) return;

        if(isFiring)
        {
            shotDuration -= Time.deltaTime;
            if(shotDuration <= 0)
            {
                shotDuration = 0.2f;
                muzzle.StartFire();
            }
        }

        //Aiming();
    }
    private void LateUpdate()
    {
        if (!weaponSetuped) return;

        Aiming();
    }
    public void SetupWeapon(Rig layerAiming)
    {
        isFiring = false;
        weaponSetuped= true;
        rigLayer_WeaponAiming = layerAiming;
    }
    public void StartFire()
    {
        isFiring = true;        
    }

    public void StopFire()
    {
        isFiring = false;
    }
    public void Aiming()
    {
        if (Input.GetMouseButton(0))
        {
            rigLayer_WeaponAiming.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            rigLayer_WeaponAiming.weight -= Time.deltaTime / aimDuration;
        }

        if (Input.GetMouseButton(0) && rigLayer_WeaponAiming.weight == 1)
        {
            StartFire();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopFire();
        }
    }
}

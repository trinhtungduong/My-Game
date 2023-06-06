using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleBaseGun : Muzzle
{
    public ParticleSystem muzzle;
    public ParticleSystem bullet;
    public override void StartFire()
    {
        base.StartFire();

        if (bullet == null || muzzle == null) return;

        if(!muzzle.isPlaying)
            muzzle.Play(true);
        if(!bullet.isPlaying)
            bullet.Play(true);
    }
    public override void StopFire()
    {
        base.StopFire();
        if(muzzle != null)
        {
            muzzle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);            
        }
        if (bullet != null)
        {
            bullet.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}

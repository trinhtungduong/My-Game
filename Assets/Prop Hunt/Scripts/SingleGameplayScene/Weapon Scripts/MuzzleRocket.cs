using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleRocket : Muzzle
{
    public ParticleSystem[] listEffects;
    public override void StartFire()
    {
        base.StartFire();

        if (listEffects.Length == 0) return;

        foreach (var particle in listEffects)
        {
            particle.Play();
        }
    }
}

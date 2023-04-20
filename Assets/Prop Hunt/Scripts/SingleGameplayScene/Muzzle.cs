using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
    public ParticleSystem[] listMuzzles;

    public void StartFire()
    {
        foreach(var particle in listMuzzles)
        {
            particle.Emit(1);
        }
    }
}

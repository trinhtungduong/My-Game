using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public ParticleSystem hitEffect;

    public void SetupHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        hitEffect.transform.position = hitPoint;
        hitEffect.transform.forward = hitNormal;        
    }

    public void SetupEffect()
    {
        hitEffect.Emit(1);
    }
}

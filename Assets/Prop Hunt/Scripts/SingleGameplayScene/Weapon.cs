using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Setting")]
    public string weaponName;
    public bool isFiring;
    public Muzzle muzzle;
    public ParticleSystem hitEffect;
    public float baseShotDuration;
    public float shotDuration;
    bool bulletOut;

    public Transform raycastOrigin;
    [HideInInspector]
    public Transform raycastDestination;
    Ray ray;
    RaycastHit hitInfo;

    private void Update()
    {
        
    }
    private void LateUpdate()
    {
        
    }

    public void StartFire()
    {
        isFiring = true;
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;
        if(Physics.Raycast(ray, out hitInfo) && bulletOut)
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
        }
    }
    public void StopFire()
    {
        isFiring = false;
        bulletOut = false;
        shotDuration = 0f;
    }
    public void UpdateFire()
    {
        if (isFiring)
        {
            shotDuration -= Time.deltaTime;
            bulletOut = false;
            if (shotDuration <= 0)
            {
                shotDuration = baseShotDuration;
                muzzle.StartFire();
                bulletOut = true;
            }
        }
    }    
}

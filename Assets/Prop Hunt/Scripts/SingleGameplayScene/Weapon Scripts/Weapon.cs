using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Setting")]
    public string weaponName;
    public Vector3 weaponOffset;
    public bool isFiring;

    [Header("Weapon Effect")]
    [SerializeField] protected Muzzle muzzle;
    [SerializeField] protected HitEffect hitEffect;

    [Header("Weapon Properties")]
    [SerializeField] protected float baseShotDuration;
    [SerializeField] protected float shotDuration;    

    [Header("Weapon Direction")]
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

    public virtual void StartFire()
    {
        isFiring = true;
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;
        if(Physics.Raycast(ray, out hitInfo))
        {
            hitEffect.SetupHit(hitInfo.point, hitInfo.normal);
        }
    }
    public virtual void StopFire()
    {
        isFiring = false;
        shotDuration = 0f;
    }
    public virtual void UpdateFire()
    {
        
    }    
}

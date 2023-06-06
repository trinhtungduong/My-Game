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
    public bool isBulletOut;
    public int bulletAmount;
    public float damage;

    [Header("Weapon Effect")]
    [SerializeField] protected Muzzle muzzle;    

    [Header("Weapon Properties")]
    [SerializeField] protected float baseShotDuration;
    [SerializeField] protected float shotDuration;    

    [Header("Weapon Direction")]
    public Transform raycastOrigin;
    [HideInInspector]
    public Transform raycastDestination;
    [HideInInspector]
    public Ray ray;
    [HideInInspector]
    public RaycastHit hitInfo;

    public virtual void StartFire()
    {
        isFiring = true;
        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;        
    }
    public virtual void StopFire()
    {
        isFiring = false;
        isBulletOut = false;
    }
    public virtual void UpdateFire()
    {
        
    }    
}

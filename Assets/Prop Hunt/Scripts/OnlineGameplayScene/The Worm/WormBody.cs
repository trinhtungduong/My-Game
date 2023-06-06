using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormBody : MonoBehaviour
{
    public bool isLook;
    public bool active;
    public ParticleSystem lazer;
    private float coolDownTime;
    private float lazerTime = 3f;
    private Quaternion lookRotation;

    private void Start()
    {
        coolDownTime = 5f;
        lazerTime = 3f;
    }
    private void Update()
    {
        if (active)
        {
            if (isLook)
            {
                coolDownTime -= Time.deltaTime;
                if(coolDownTime < 0f)
                {
                    isLook = false;
                    coolDownTime = 10f;
                    lazer.Play(true);
                }
            }
            else
            {
                lazerTime -= Time.deltaTime;
                if(lazerTime < 0f)
                {
                    lazerTime = 3f;
                    isLook = true;
                    lazer.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
    }
    public void LookAt(Vector3 target)
    {
        if (isLook)
        {
            lookRotation = Quaternion.LookRotation(target - transform.position);         
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);        
        }
        else
        {
            transform.rotation = lookRotation;
        }
    }
    
}

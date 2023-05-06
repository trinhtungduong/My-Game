using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigAnimator : MonoBehaviour
{
    [Header("Rig Reference")]
    public Animator animator;
    public TwoBoneIKConstraint leftHandIK;
    public Rig rigHand;
    public MultiPositionConstraint positionConstraint;

    [Header("Rig Builder")]
    public RigBuilder rigBuilder;

    [Header("Variables")]
    private bool isLeftHand;
    private float timeLeft;
    private float timeBase;

    private void Update()
    {
        if (isLeftHand)
        {
            timeLeft -= Time.deltaTime;
            timeLeft = Mathf.Clamp(timeLeft, 0f, timeBase);
            leftHandIK.weight = 1f - timeLeft / timeBase;

            if(timeLeft == 0f)
            {
                isLeftHand = false;
            }
        }
    }
    public void InitRig(float rigHandWeight, float leftHandIKWeight)
    {
        rigHand.weight = rigHandWeight;
        leftHandIK.weight = leftHandIKWeight;

        isLeftHand = false;
        timeLeft = 0f;
        timeBase = 0f;
    }
    public void PlayAnimation(string nameOfAnimation)
    {
        animator.Play(nameOfAnimation);        
    }
    public float GetSwitchGunTime()
    {
        return animator.GetCurrentAnimatorClipInfo(0).Length;
    }
    public void Event_LeftWeight(float time)
    {
        timeLeft = time;
        timeBase = time;
        isLeftHand = true;
    }
    public void WeaponOffset(Vector3 offset)
    {
        positionConstraint.data.offset = offset;
        rigBuilder.Build();
    }
}

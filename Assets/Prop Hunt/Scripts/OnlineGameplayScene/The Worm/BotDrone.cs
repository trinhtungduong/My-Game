using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDrone : MonoBehaviour
{
    public bool isActive;
    public Transform aim;
    [HideInInspector]
    public Transform target;
    private Vector3 direction;
    private Vector3 endPos;
    public void Active()
    {
        target = MapManager.instance.GetRandomTarget();
        isActive = true;
    }
    //private void Update()
    //{
    //    if (isActive)
    //    {
    //        MoveToTarget();
    //    }
    //}
    public void MoveToTarget()
    {
        if(target != null)
        {
            aim.LookAt(target);
            direction = (transform.position - target.position).normalized;
            endPos = target.position + direction * 3f;
            transform.position = Vector3.MoveTowards(transform.position, endPos - Vector3.up * (endPos.y - 1.5f), Time.deltaTime);
        }
    }
}

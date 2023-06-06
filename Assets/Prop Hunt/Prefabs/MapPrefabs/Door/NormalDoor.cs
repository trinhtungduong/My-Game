using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDoor : DoorTrigger
{
    public override void TriggerAction()
    {
        base.TriggerAction();
        door.doorAnimator.Play("OpenDoor");
        door.nextRoom.InstatiateRoom(SingleMapManager.Instance.RandomNextRoom());
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerAction();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public TypeItem typeItem;
    public int amount;
    [HideInInspector]
    public SinglePlayerController player;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PickRay") && player == null)
        {
            Debug.Log("TRIGGER ITEM");
            if (Input.GetKeyDown(KeyCode.E))
            {
                player = other.GetComponent<PickRay>().player;
                Destroy(GetComponent<Collider>());
                player.PickItem(this);
            }
        }
    }
}

public enum TypeItem
{
    Key,
    Vitamin
}

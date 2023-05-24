using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.LogWarning("FIRE ENTER");
            other.GetComponent<IDamagePlayer>()?.TakeDamage();
            gameObject.SetActive(false);
        }
    }
}

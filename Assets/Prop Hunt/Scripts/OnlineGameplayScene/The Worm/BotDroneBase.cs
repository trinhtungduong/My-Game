using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotDroneBase : MonoBehaviour
{
    public List<BotDrone> listBotDrones;
    public Transform spawnPosition;

    public void Spawn()
    {
        foreach(var drone in listBotDrones)
        {
            if(drone != null)
            {
                if (!drone.gameObject.activeInHierarchy)
                {
                    drone.transform.position = spawnPosition.position;
                    drone.transform.rotation = spawnPosition.rotation;
                    drone.Active();
                    drone.gameObject.SetActive(true);
                }
            }
        }
    }
}

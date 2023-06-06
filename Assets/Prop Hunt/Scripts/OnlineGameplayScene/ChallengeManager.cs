using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public bool challengeActive;
    public List<BotDroneBase> droneBases;

    public void Active()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < droneBases.Count; i++)
            {
                droneBases[j].Spawn();
            }
        }
        challengeActive = true;
    }

    private void Update()
    {

    }
}

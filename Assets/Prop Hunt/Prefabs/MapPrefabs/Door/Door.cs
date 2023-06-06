using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [HideInInspector]
    public RoomInMap nextRoom;
    public Animator doorAnimator;    
}

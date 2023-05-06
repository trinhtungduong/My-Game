using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;
    public Transform crosshairTarget;
    public Transform aimLookAt;

    private void Awake()
    {
        Instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance;
    public FloatingJoystick moveJoystick;
    public FloatingJoystick cameraJoystick;
    public Transform mainCameraTransform;

    private void Awake()
    {
        Instance = this;
    }
}

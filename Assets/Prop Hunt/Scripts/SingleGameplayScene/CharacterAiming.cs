using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    [Header("Camera Setting")]
    //[SerializeField] AxisState xAxis;
    //[SerializeField] AxisState yAxis;
    float xAxis;
    float yAxis;
    [SerializeField] Transform cameraLookAt;
    [SerializeField] float turnSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        xAxis += Input.GetAxis("Mouse X") * 2f;
        yAxis -= Input.GetAxis("Mouse Y") * 3f;
        yAxis = Mathf.Clamp(yAxis, -60f, 60f);
    }
    private void FixedUpdate()
    {

    }
    private void LateUpdate()
    {
        MoveCameraAround();
    }

    public void MoveCameraAround()
    {
        cameraLookAt.localEulerAngles = new Vector3(yAxis, cameraLookAt.localEulerAngles.y, cameraLookAt.localEulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, xAxis, transform.rotation.z), turnSpeed * Time.deltaTime);
    }    
}

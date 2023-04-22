using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    [Header("Camera Setting")]
    //[SerializeField] CinemachineFreeLook thirdPersonCamera;    
    [SerializeField] Transform mainCameraTransform;
    //[SerializeField] float offsetY;    
    [SerializeField] AxisState xAxis;
    [SerializeField] AxisState yAxis;
    [SerializeField] Transform cameraLookAt;

    [Header("Camera Distance")]
    [SerializeField] CinemachineVirtualCamera cinemachine3rdPersonFollow;
    Cinemachine3rdPersonFollow thirdPersonFollow;
    float cameraDst;
    [SerializeField] float zoomSpeed;
    bool isZoomIn;
    bool isZoomOut;

    [Header("Weapon")]
    public Weapon weapon;
    public Rig rigLayer_WeaponAiming;
    public float aimDuration;
    // Start is called before the first frame update
    void Start()
    {
        thirdPersonFollow = cinemachine3rdPersonFollow.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        isZoomIn = false;
        isZoomOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Aiming();
        //ZoomInOut();
    }
    private void FixedUpdate()
    {
        MoveCameraAround();
    }
    private void LateUpdate()
    {
        Aiming();
        //ZoomInOut();
    }

    public void MoveCameraAround()
    {
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);

        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0f);

        var angleRouting = mainCameraTransform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angleRouting, 0), 0.2f);
    }
    public void Aiming()
    {
        if (Input.GetMouseButton(0))
        {
            rigLayer_WeaponAiming.weight += Time.deltaTime / aimDuration;
        }
        else
        {
            rigLayer_WeaponAiming.weight -= Time.deltaTime / aimDuration;
        }

        if (Input.GetMouseButton(0) && rigLayer_WeaponAiming.weight == 1)
        {
            if (weapon)
            {
                weapon.StartFire();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            isZoomIn = true;
            isZoomOut = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (weapon)
            {
                weapon.StopFire();
            }
            isZoomIn = false;
            isZoomOut = true;
        }
    }
    public void ZoomInOut()
    {
        if (isZoomIn)
        {
            cameraDst -= Time.deltaTime * zoomSpeed;
            cameraDst = Mathf.Clamp(cameraDst, 2f, 3f);
            thirdPersonFollow.CameraDistance = cameraDst;
        }

        if (isZoomOut)
        {
            cameraDst += Time.deltaTime * zoomSpeed;
            cameraDst = Mathf.Clamp(cameraDst, 2f, 3f);
            thirdPersonFollow.CameraDistance = cameraDst;
        }
    }
}

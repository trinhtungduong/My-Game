using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Setting")]
    Vector3 velocity;
    Vector3 gravity;
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gValue;
    [SerializeField] bool isOnGround;
    float turnSmoothVelocity;
    [SerializeField] float rotationX;
    [SerializeField] float rotationY;
    [SerializeField] CinemachineFreeLook thirdPersonCamera;
    Transform mainCameraTransform;
    [SerializeField] Transform groundDetect;

    [Header("Layer Setting")]
    [SerializeField] LayerMask surface;

    [Header("Joystick")]
    FloatingJoystick moveJoystick;
    FloatingJoystick cameraJoystick;

    [Header("Physics Setting")]
    //public Rigidbody rigidBody;
    public CharacterController character;

    [Header("Photon Setting")]
    public PhotonView photonView;

    private void Start()
    {
        moveJoystick = InputController.Instance.moveJoystick;
        cameraJoystick = InputController.Instance.cameraJoystick;
        mainCameraTransform = InputController.Instance.mainCameraTransform;
        if (!photonView.IsMine)
        {
            Destroy(thirdPersonCamera.gameObject);
        }
    }
    private void Update()
    {
        if (!photonView.IsMine) return;

        MoveCameraAround();

        float horizontal = moveJoystick.Horizontal;
        float vertical = moveJoystick.Vertical;
        float targetAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg + mainCameraTransform.eulerAngles.y;
            
        Rotate(horizontal, vertical, targetAngle);
        MoveCalculate(horizontal, vertical, targetAngle);
        Fall();
    }
    private void FixedUpdate()
    {
        //if (!photonView.IsMine) return;

        //Move();
        //Fall();
    }

    public void Rotate(float horizontal, float vertical, float targetAngle)
    {
        if (horizontal == 0 && vertical == 0) return;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);        
    }
    public void MoveCalculate(float horizontal, float vertical, float targetAngle)
    {
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        if (horizontal == 0f && vertical == 0f)
            moveDir = Vector3.zero;

        velocity = moveDir.normalized * moveSpeed;
        character.Move(velocity * Time.deltaTime);                  
    }
    public void Fall()
    {
        if (!CheckOnGround())
        {
            //rigidBody.AddForce(-transform.up * fallForce);
            gravity.y += gValue * Time.deltaTime;
        }
        else
        {
            gravity.y = 0f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //rigidBody.AddForce(transform.up * jumpForce);
                gravity.y += Mathf.Sqrt(jumpHeight * -2f * gValue);                
            }
        }

        character.Move(gravity * Time.deltaTime);
    }
    public void MoveCameraAround()
    {
        rotationY += cameraJoystick.Horizontal * 150f * Time.deltaTime;
        rotationX += cameraJoystick.Vertical * (-1) * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, 0f, 1f);
        thirdPersonCamera.m_XAxis.Value = rotationY;
        thirdPersonCamera.m_YAxis.Value = rotationX;
    }
    public bool CheckOnGround()
    {
        isOnGround = Physics.CheckSphere(groundDetect.position, 0.5f, surface);
        return isOnGround;
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerController : MonoBehaviour
{
    [Header("Movement Setting")]    
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gValue;
    [SerializeField] bool isOnGround;
    [SerializeField] float rotationX;
    [SerializeField] float rotationY;    
    [SerializeField] Transform groundDetect;
    Vector3 velocity;
    Vector3 gravity;

    [Header("Camera Setting")]
    [SerializeField] CinemachineFreeLook thirdPersonCamera;
    [SerializeField] Transform mainCameraTransform;

    [Header("Layer Setting")]
    [SerializeField] LayerMask surface;

    [Header("Physics Setting")]
    //public Rigidbody rigidBody;
    public CharacterController character;

    [Header("Animator Setting")]
    public Animator animator;

    private void Start()
    {

    }
    private void Update()
    {
        MoveCameraAround();
        MoveCalculate();
        Fall();
    }
    public void MoveCalculate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float targetAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg + mainCameraTransform.eulerAngles.y;

        animator.SetFloat("InputX", Input.GetAxis("Horizontal"));
        animator.SetFloat("InputY", Input.GetAxis("Vertical"));

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
                animator.SetTrigger("Jump");
            }
        }

        character.Move(gravity * Time.deltaTime);
    }
    public void MoveCameraAround()
    {
        var angleRouting = mainCameraTransform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, angleRouting, 0), Time.deltaTime * 100f);
    }
    public bool CheckOnGround()
    {
        isOnGround = Physics.CheckSphere(groundDetect.position, 0.5f, surface);
        return isOnGround;
    }
}

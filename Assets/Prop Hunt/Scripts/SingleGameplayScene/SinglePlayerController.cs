using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SinglePlayerController : MonoBehaviour
{
    [Header("Movement Setting")]    
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] float gValue;
    [SerializeField] bool isJumping;
    [SerializeField] float stepDown;
    //[SerializeField] float rotationX;
    //[SerializeField] float rotationY;    
    [SerializeField] Transform groundDetect;
    Vector3 velocity;
    Vector3 gravity;

    [Header("Camera Setting")]
    //[SerializeField] CinemachineFreeLook thirdPersonCamera;
    [SerializeField] Transform mainCameraTransform;

    [Header("Layer Setting")]
    [SerializeField] LayerMask surface;

    [Header("Physics Setting")]
    //public Rigidbody rigidBody;
    public CharacterController characterController;

    [Header("Animator Setting")]
    public bool animatorSetuped;
    public Animator animator;
    public Vector3 rootMotion;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        MoveCalculate();
        LocoMotion();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    private void FixedUpdate()
    {
        Move();        
        //Fall();
    }
    public void LocoMotion()
    {
        animator.SetFloat("InputX", Input.GetAxis("Horizontal"));
        animator.SetFloat("InputY", Input.GetAxis("Vertical"));
    }
    public void MoveCalculate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = transform.right * horizontal + transform.forward * vertical;
        velocity = moveDir.normalized * moveSpeed;
    }
    public void Move()
    {
        characterController.Move(velocity * Time.fixedDeltaTime);
    }
    
    public void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;
            gravity = animator.velocity;
            gravity.y = Mathf.Sqrt(jumpHeight * -2f * gValue);
        }
    }
  
    public bool CheckOnGround()
    {        
        return Physics.CheckSphere(groundDetect.position, 0.5f, surface);
    }
}

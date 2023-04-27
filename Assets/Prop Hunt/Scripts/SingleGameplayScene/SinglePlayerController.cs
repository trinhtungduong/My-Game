using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SinglePlayerController : MonoBehaviour
{
    [Header("Movement Setting")]
    public CharacterController characterController;    
    public float moveSpeed = 3f;
    [HideInInspector]
    public Vector3 dir;
    float horizontalInput;
    float verticalInput;

    [Header("Gravity Setting")]
    //[SerializeField] Transform groundDetect;
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask surface;
    Vector3 spherePos;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] bool isGrounded;
    Vector3 gravityVelocity;

    [Header("Animator")]
    [SerializeField] Animator characterAnimator;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        GetDirectionAndMove();
        GravityApply();
        LocoMotion();
    }
    private void FixedUpdate()
    {
        //GetDirectionAndMove();
        //GravityApply();
    }
    public void LocoMotion()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");

        characterAnimator.SetFloat("InputX", inputX);
        characterAnimator.SetFloat("InputY", inputY);
    }
    public void GetDirectionAndMove()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        dir = transform.forward * verticalInput + transform.right * horizontalInput;
        characterController.Move(dir * moveSpeed * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, characterController.radius - 0.05f, surface)) return true;
        return false;
    }

    public void GravityApply()
    {
        isGrounded = IsGrounded();
        if (!isGrounded) gravityVelocity.y += gravity * Time.deltaTime;
        else
        {
            gravityVelocity.y = 0f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gravityVelocity.y += 5f;
            }
        }

        characterController.Move(gravityVelocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePos, characterController.radius - 0.05f);
    }
}

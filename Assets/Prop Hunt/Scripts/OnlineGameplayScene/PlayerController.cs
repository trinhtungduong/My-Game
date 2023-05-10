using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement Setup")]
    public CharacterController characterController;
    public float moveSpeed = 3f;
    public float jumpHeight = 10f;
    [HideInInspector]
    public Vector3 dir;
    float horizontalInput;
    float verticalInput;

    [Header("Gravity Setup")]
    //[SerializeField] Transform groundDetect;
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask surface;
    Vector3 spherePos;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] bool isGrounded;
    Vector3 gravityVelocity;

    [Header("Animator")]
    [SerializeField] Animator characterAnimator;

    [Header("Camera Setting")]
    [SerializeField] GameObject thirdPersonCamera;
    [SerializeField] Transform cameraLookAt;
    [SerializeField] float turnSpeed;
    [SerializeField] float sensitivity;
    float xAxis;
    float yAxis;    

    [Header("Gun Setup")]
    public Transform crossHairTarget;
    [HideInInspector]
    public Weapon weapon;    
    public RigAnimator rigController;
    public int indexWeapon;
    public List<Weapon> listWeapons;
    public WeaponEquipData weaponEquipData;
    public Transform weaponPivot;
    [HideInInspector]
    public bool isSwitching;
    [HideInInspector]
    public float timeSwitching;
    public bool isInitWeapon;
    public bool isCheckingEquip;

    [Header("Rig Setup")]
    public int iterations;
    public float angleLimit = 90f;
    public Transform aimTransform;
    public Transform targetTransform;
    public Transform headBone;
    public Transform neckBone;
    public Transform weaponHolder;

    [Header("Photon Setup")]
    [SerializeField] PhotonView PV;

    private void Start()
    {
        InitListWeapon();

        if (PV.IsMine)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Application.targetFrameRate = 60;

            neckBone.localScale = Vector3.zero;

            crossHairTarget = InputController.Instance.crosshairTarget;
            
            indexWeapon = 0;
            ChangeGun();
        }
        else
        {
            Destroy(thirdPersonCamera.gameObject);
        }
    }
    private void Update()
    {
        if (!PV.IsMine)
        {
            if (isCheckingEquip)
            {
                if (isInitWeapon)
                {
                    isCheckingEquip = false;
                    ChangeGun();
                }
            }
            return;
        }

        GetDirectionAndMove();
        GravityApply();
        LocoMotion();

        UpdateInput();

        if (isSwitching)
        {
            timeSwitching -= Time.deltaTime;
            if (timeSwitching <= 0f)
            {
                isSwitching = false;
            }
        }
        Aiming();
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeGun();
        }
    }
    private void FixedUpdate()
    {

    }
    private void LateUpdate()
    {
        if (PV.IsMine)
        {
            weaponHolder.rotation = Quaternion.Slerp(weaponHolder.rotation, cameraLookAt.rotation, Time.deltaTime * turnSpeed);
            MoveCameraAround();
        }
        else
        {
            AimBones();
        }            
    }
    #region Player Controller
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
                gravityVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        characterController.Move(gravityVelocity * Time.deltaTime);
    }
    #endregion

    #region Player Aiming
    public void UpdateInput()
    {
        xAxis += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        yAxis -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        yAxis = Mathf.Clamp(yAxis, -45f, 45f);
    }
    public void MoveCameraAround()
    {
        cameraLookAt.localEulerAngles = new Vector3(yAxis, cameraLookAt.localEulerAngles.y, cameraLookAt.localEulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.x, xAxis, transform.rotation.z), turnSpeed * Time.deltaTime);       
    }
    #endregion

    #region Active Weapon
    public void Aiming()
    {
        if (weapon && !isSwitching)
        {
            if (Input.GetMouseButtonDown(0))
            {
                weapon.StartFire();
            }
            weapon.UpdateFire();
            if (Input.GetMouseButtonUp(0))
            {
                weapon.StopFire();
            }
        }
    }
    public void InitListWeapon()
    {
        listWeapons.Clear();
        for(int i = 0; i < weaponEquipData.listWeapons.Count; i++)
        {
            var newWeapon = Instantiate(weaponEquipData.listWeapons[i], weaponPivot);
            listWeapons.Add(newWeapon);
        }
        isInitWeapon = true;
    }
    public void Equip(Weapon newWeapon)
    {
        if (weapon == newWeapon) return;

        if (weapon)
        {
            weapon.gameObject.SetActive(false);
        }
        isSwitching = false;
        weapon = newWeapon;
        weapon.raycastDestination = crossHairTarget;
        weapon.gameObject.SetActive(true);
        rigController.InitRig(1f, 0f);
        rigController.WeaponOffset(weapon.weaponOffset);
        rigController.PlayAnimation("equip_" + weapon.weaponName);
        timeSwitching = rigController.GetSwitchGunTime();
        //timeSwitching = 0.5f;
        isSwitching = true;        
    }
    public void ChangeGun()
    {
        Equip(listWeapons[indexWeapon]);

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("indexWeapon", indexWeapon);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }

        indexWeapon = (indexWeapon >= (listWeapons.Count - 1)) ? 0 : (indexWeapon + 1);      
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (isInitWeapon)
            {
                Equip(listWeapons[(int)changedProps["indexWeapon"]]);
            }
            else
            {
                isCheckingEquip = true;
            }
        }
    }
    #endregion

    #region Rig Controller
    //public void RigAimSetup()
    //{
    //    foreach (var component in listAims)
    //    {
    //        var data = component.data.sourceObjects;
    //        data.SetTransform(0, InputController.Instance.aimLookAt);
    //        component.data.sourceObjects = data;
    //    }
    //    rigBuilder.Build();
    //}
    public void AimBones()
    {
        for(int i = 0; i < iterations; i++)
        {
            AimAtTarget(headBone, GetTargetPosition());
        }
        weaponHolder.rotation = aimTransform.rotation;
    }
    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = targetTransform.position - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;

        float blendOut = 0.0f;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);

        if(targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }
    public void AimAtTarget(Transform bone, Vector3 targetPosition)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimToward = Quaternion.FromToRotation(aimDirection, targetDirection);
        bone.rotation = aimToward * bone.rotation;
    }
    #endregion
}
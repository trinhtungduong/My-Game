using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IDamagePlayer
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
    bool prevOnGround;
    Vector3 gravityVelocity;
    float velocityY;

    [Header("Animator")]
    [SerializeField] Animator characterAnimator;

    [Header("Camera Setting")]
    [SerializeField] CinemachineVirtualCamera thirdPersonCamera;
    [SerializeField] Transform cameraLookAt;
    [SerializeField] float turnSpeed;
    [SerializeField] float sensitivity;

    [Header("Crosshair Setup")]
    public Transform crossHairTarget;
    Camera mainCamera;
    Ray ray;
    RaycastHit hitInfo;
    float xAxis;
    float yAxis;

    [Header("Gun Setup")]   
    public RigAnimator rigController;
    [HideInInspector]
    public Weapon weapon;
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
    public Transform weaponHolder;

    [Header("Photon Setup")]
    [SerializeField] PhotonView PV;
    PlayerManager playerManager;

    [Header("Player Properties")]
    public float health = 1000f;
    public bool isAlive;
    public bool damaging;
    private void Awake()
    {
        if(!PhotonNetwork.OfflineMode)
            playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }
    private void Start()
    {
        InitListWeapon();

        isAlive = true;

        if (PV.IsMine)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Application.targetFrameRate = 60;

            mainCamera = Camera.main;
            indexWeapon = 0;
            ChangeGun();

            thirdPersonCamera.Priority = 10;          
        }
        else
        {
            thirdPersonCamera.Priority = 5;
            Destroy(characterController);
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

        if (!isAlive) return;

        GetDirectionAndMove();
        GravityApply();
        LocoMotion();

        UpdateCrosshair();
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
            AimBones();
            MoveCameraAround();
        }
        else
        {
            AimBones();
        }
    }
    #region Player Controller
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
        prevOnGround = isGrounded;
        isGrounded = IsGrounded();
        if (!isGrounded)
        {
            gravityVelocity.y += gravity * Time.deltaTime;
            velocityY = Mathf.Clamp(velocityY - Time.deltaTime * 5f, -1f, 1f);
            characterAnimator.SetFloat("VelocityY", velocityY);
        }
        else
        {
            if (!prevOnGround)
            {
                PV.RPC(nameof(RPC_SetTrigger), RpcTarget.All, "OnGround");
            }

            gravityVelocity.y = 0f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gravityVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
                PV.RPC(nameof(RPC_SetTrigger), RpcTarget.All, "Jump");
                velocityY = 1f;
            }
        }

        characterController.Move(gravityVelocity * Time.deltaTime);
    }
    public void RemovePlayer()
    {
        playerManager.RemovePlayer();
    }
    #endregion

    #region Player Animator
    public void LocoMotion()
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");

        characterAnimator.SetFloat("InputX", inputX);
        characterAnimator.SetFloat("InputY", inputY);
    }

    public void ResetAllTriggers()
    {
        foreach(var param in characterAnimator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
                characterAnimator.ResetTrigger(param.name);
        }
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
    public void UpdateCrosshair()
    {
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;
        Physics.Raycast(ray, out hitInfo);
        crossHairTarget.transform.position = hitInfo.point;
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
            newWeapon.gameObject.SetActive(false);
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
        weaponHolder.rotation = Quaternion.Slerp(weaponHolder.rotation, cameraLookAt.rotation, Time.deltaTime * turnSpeed);
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

    #region Photon Interface
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
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
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        MapManager.instance.AddNewPlayer(this);
        //info.Sender.TagObject = gameObject;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(!PV.IsMine && PV.Owner == otherPlayer)
        {
            MapManager.instance.InActivePlayer(this);
        }
    }
    [PunRPC]
    void RPC_SetTrigger(string nameofTrigger)
    {
        ResetAllTriggers();
        characterAnimator.SetTrigger(nameofTrigger);
    }
    [PunRPC]
    void RPC_TakeDamage()
    {
        damaging = true;
        Invoke(nameof(ResetDamaging), 3f);
        health -= 400f;
        if(PV.IsMine)
            MapManager.instance.UpdatePlayerHealth(health, 1000f);

        if (health <= 0f)
        {
            isAlive = false;
            if (PV.IsMine)
            {
                thirdPersonCamera.Priority = 0;               
            }
            MapManager.instance.UpdateAlivePlayer();
        }
    }
    #endregion

    #region Interface
    public void TakeDamage()
    {
        if(!damaging)
            PV.RPC(nameof(RPC_TakeDamage), RpcTarget.All);
    }
    public void ResetDamaging()
    {
        damaging = false;
    }
    #endregion
}

using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SinglePlayerController : MonoBehaviour
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

    [Header("Item")]
    public Transform itemHolder;
    [HideInInspector]
    public Item currentItem;    
    public List<Item> listItems;

    [Header("Rig Setup")]
    public int iterations;
    public float angleLimit = 90f;
    public Transform aimTransform;
    public Transform targetTransform;
    public Transform headBone;
    public Transform weaponHolder;

    [Header("Player Properties")]
    public float health = 1000f;
    public bool isAlive;
    public bool damaging;
    private void Start()
    {
        InitListWeapon();

        isAlive = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Application.targetFrameRate = 60;

        mainCamera = Camera.main;
        indexWeapon = 0;
        ChangeGun();

        thirdPersonCamera.Priority = 10;
    }
    private void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeGun();
        }
        for(int i = 0; i < listItems.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
            }
        }
    }
    private void FixedUpdate()
    {

    }
    private void LateUpdate()
    {
        AimBones();
        MoveCameraAround();
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
                RPC_SetTrigger("OnGround");
            }

            gravityVelocity.y = 0f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gravityVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
                RPC_SetTrigger("Jump");
                velocityY = 1f;
            }
        }

        characterController.Move(gravityVelocity * Time.deltaTime);
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
        foreach (var param in characterAnimator.parameters)
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
        for (int i = 0; i < weaponEquipData.listWeapons.Count; i++)
        {
            var newWeapon = Instantiate(weaponEquipData.listWeapons[i], weaponPivot);
            newWeapon.gameObject.SetActive(false);
            listWeapons.Add(newWeapon);
        }
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
        isSwitching = true;
    }
    public void ChangeGun()
    {
        if (currentItem)
        {
            currentItem = null;
            Equip(listWeapons[indexWeapon]);
        }
        else
        {
            Equip(listWeapons[indexWeapon]);
            indexWeapon = (indexWeapon >= (listWeapons.Count - 1)) ? 0 : (indexWeapon + 1);
        }        
    }
    #endregion

    #region Items
    public void EquipItem(int index)
    {
        if (currentItem)
        {
            currentItem.gameObject.SetActive(false);
        }
        currentItem = listItems[index];
        currentItem.gameObject.SetActive(true);
        //rigController.PlayAnimation("equip_item");
    }
    public void PickItem(Item newItem)
    {
        for(int i = 0; i < listItems.Count; i++)
        {
            if(CheckTypeItemMultiple(listItems[i].typeItem) && listItems[i].typeItem == newItem.typeItem)
            {
                listItems[i].amount += 1;
                Destroy(newItem.gameObject);
                //Update UI;
                break;
            }
        }

        listItems.Add(newItem);
        newItem.transform.SetParent(itemHolder, true);
        newItem.gameObject.SetActive(false);
        //Update UI;
    }
    public bool CheckTypeItemMultiple(TypeItem type)
    {
        return type == TypeItem.Vitamin;
    }
    #endregion
    #region Rig Controller
    public void AimBones()
    {
        for (int i = 0; i < iterations; i++)
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

        if (targetAngle > angleLimit)
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

    void RPC_SetTrigger(string nameofTrigger)
    {
        ResetAllTriggers();
        characterAnimator.SetTrigger(nameofTrigger);
    }
    void RPC_TakeDamage()
    {
        damaging = true;
        Invoke(nameof(ResetDamaging), 3f);
        health -= 400f;
        MapManager.instance.UpdatePlayerHealth(health, 1000f);

        if (health <= 0f)
        {
            isAlive = false;
            thirdPersonCamera.Priority = 0;
            MapManager.instance.UpdateAlivePlayer();
        }
    }

    #region Interface
    public void TakeDamage()
    {
        RPC_TakeDamage();
    }
    public void ResetDamaging()
    {
        damaging = false;
    }
    #endregion
}
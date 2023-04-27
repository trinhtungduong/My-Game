using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public Transform crossHairTarget;
    [HideInInspector]
    public Weapon weapon;

    public RigAnimator rigController;
    public int indexWeapon;
    public List<Weapon> listWeapons;
    [HideInInspector]
    public bool isSwitching;
    [HideInInspector]
    public float timeSwitching;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwitching)
        {
            timeSwitching -= Time.deltaTime;
            if(timeSwitching <= 0f)
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
    public void Aiming()
    {
        if (weapon && !isSwitching)
        {
            if (Input.GetMouseButton(0))
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
        rigController.PlayAnimation("equip_" + weapon.weaponName);
        timeSwitching = rigController.GetSwitchGunTime();
        //timeSwitching = 0.5f;
        isSwitching = true;
    }

    public void ChangeGun()
    {       
        Equip(listWeapons[indexWeapon]);
        indexWeapon = (indexWeapon >= (listWeapons.Count - 1)) ? 0 : (indexWeapon + 1);
    }
}

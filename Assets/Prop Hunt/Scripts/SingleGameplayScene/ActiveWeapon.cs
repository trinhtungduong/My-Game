using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public Transform crossHairTarget;
    public Weapon weapon;
    public UnityEngine.Animations.Rigging.Rig handIK;

    //public Animator rigController;
    public Weapon weaponPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Aiming();
        if (Input.GetKeyDown(KeyCode.K))
        {
            Equip(weaponPrefab);
        }
    }
    public void Aiming()
    {
        if (weapon)
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
        //if (weapon == newWeapon) return;

        if (weapon)
        {
            weapon.gameObject.SetActive(false);
        }
        weapon = newWeapon;
        weapon.raycastDestination = crossHairTarget;
        weapon.gameObject.SetActive(true);
        //rigController.Play("equip_" + weapon.weaponName);
    }
}

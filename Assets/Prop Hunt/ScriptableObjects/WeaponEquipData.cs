using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Equip", menuName = "Weapon Equip")]
public class WeaponEquipData : ScriptableObject
{
    public List<Weapon> listWeapons;
}

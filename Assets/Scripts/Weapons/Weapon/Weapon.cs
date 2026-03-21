using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon 
{
    public WeaponDetailsSO weaponDetails;
    public int weaponListPosition;
    public float weaponReloadTimer; //武器重新装填计时器
    public int weaponClipRemainingAmmo; //武器弹夹中剩余的弹药数量
    public int weaponRemainingAmmo; //武器总剩余弹药量
    public bool isWeaponReloading; //武器是否处于重新装填状态中
}

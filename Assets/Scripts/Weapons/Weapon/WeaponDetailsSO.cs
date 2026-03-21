using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapons/Weapon Details")]
public class WeaponDetailsSO : ScriptableObject
{
    #region Header WEAPON BASE DETAILS
    [Space(10)]
    [Header("武器基础信息")]
    #endregion Header WEAPON BASE DETAILS
    #region Tooltip
    [Tooltip("武器名")]
    #endregion Tooltip
    public string weaponName;

    #region Tooltip
    [Tooltip("武器的精灵图 - 精灵图应勾选“生成物理形状”选项")]
    #endregion Tooltip
    public Sprite weaponSprite;

    #region Header WEAPON CONFIGURATION
    [Space(10)]
    [Header("武器配置")]
    #endregion Header WEAPON CONFIGURATION
    #region Tooltip
    [Tooltip("武器射击位置 - 武器枪口相对于精灵枢轴点的偏移位置")]
    #endregion Tooltip
    public Vector3 weaponShootPosition;

    #region Tooltip
    [Tooltip("武器当前弹药")]
    #endregion Tooltip
    public AmmoDetailsSO weaponCurrentAmmo;

    [Space(10)]
    #region Tooltip
    [Tooltip("武器射击效果SO - 包含与武器射击效果预制体结合使用的粒子效果参数")]
    #endregion Tooltip
    public WeaponShootEffectSO weaponShootEffect;

    #region Tooltip
    [Tooltip("武器的射击音效")]
    #endregion Tooltip
    public SoundEffectSO weaponFiringSoundEffect;

    #region Tooltip
    [Tooltip("武器的装弹音效SO")]
    #endregion Tooltip
    public SoundEffectSO weaponReloadingSoundEffect;

    #region Header WEAPON OPERATING VALUES
    [Space(10)]
    [Header("武器操作值")]
    #endregion Header WEAPON OPERATING VALUES
    #region Tooltip
    [Tooltip("选择武器是否有无限弹药")]
    #endregion Tooltip
    public bool hasInfiniteAmmo = false;

    #region Tooltip
    [Tooltip("选择武器是否具有无限弹夹容量")]
    #endregion Tooltip
    public bool hasInfiniteClipCapacity = false;

    #region Tooltip
    [Tooltip("武器弹夹容量 - shots before a reload")]
    #endregion Tooltip
    public int weaponClipAmmoCapacity = 6;

    #region Tooltip
    [Tooltip("武器的总弹药容量 - the maximum number of rounds at that can be held for this weapon")]
    #endregion Tooltip
    public int weaponAmmoCapacity = 100;

    #region Tooltip
    [Tooltip("武器射击速率 - 0.2 代表 5 发子弹每秒（1 / 5）")]
    #endregion Tooltip
    public float weaponFireRate = 0.2f;

    #region Tooltip
    [Tooltip("武器预充能时间-射击前按下射击按钮的时间（秒）")]
    #endregion Tooltip
    public float weaponPrechargeTime = 0f;

    #region Tooltip
    [Tooltip("武器装填时间（秒）")]
    #endregion Tooltip
    public float weaponReloadTime = 0f;

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponCurrentAmmo), weaponCurrentAmmo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }
    }

#endif
    #endregion Validation
}

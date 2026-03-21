using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    #region Header PLAYER BASE DETAILS
    [Space(10)]
    [Header("玩家基础信息")]
    #endregion
    #region Tooltip
    [Tooltip("玩家名称")]
    #endregion
    public string playerCharacterName;

    #region Tooltip
    [Tooltip("玩家预制件")]
    #endregion
    public GameObject playerPrefab;

    #region Tooltip
    [Tooltip("玩家运行时动画控制器")]
    #endregion
    public RuntimeAnimatorController runtimeAnimatorController;

    #region Header HEALTH
    [Space(10)]
    [Header("生命值")]
    #endregion
    #region Tooltip
    [Tooltip("玩家初始生命值")]
    #endregion
    public int playerHealthAmount;

    #region Tooltip
    [Tooltip("选择是否在被击中后立即有免疫期。如果是这样，请在另一个字段中指定免疫时间（秒）")]
    #endregion
    [HideInInspector] public bool isImmuneAfterHit = false;

    #region Tooltip
    [Tooltip("被击中后的免疫时间（秒）")]
    #endregion
    [HideInInspector] public float hitImmunityTime;

    #region Header WEAPON
    [Space(10)]
    [Header("武器")]
    #endregion
    #region Tooltip
    [Tooltip("玩家初始启动武器")]
    #endregion
    public WeaponDetailsSO startingWeapon;

    #region Tooltip
    [Tooltip("填充起始武器列表")]
    #endregion
    public List<WeaponDetailsSO> startingWeaponList;

    #region Header OTHER
    [Space(10)]
    [Header("其他")]
    #endregion
    #region Tooltip
    [Tooltip("小地图中使用的玩家图标精灵")]
    #endregion
    public Sprite playerMiniMapIcon;

    #region Tooltip
    [Tooltip("玩家手部精灵")]
    #endregion
    public Sprite playerHandSprite;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);

        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
#endif
    #endregion
}

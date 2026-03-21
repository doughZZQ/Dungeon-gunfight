using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }

            return instance;
        }
    }

    #region Header 地牢
    [Space(10)]
    [Header("地牢")]
    #endregion
    #region 工具提示
    [Tooltip("用地牢的 RoomNodeTypeListSO 进行填充")]
    #endregion
    public RoomNodeTypeListSO roomNodeTypeList;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion Header PLAYER
    #region Tooltip
    [Tooltip("玩家详细信息列表-用玩家详细信息可脚本化对象填充列表")]
    #endregion Tooltip
    public List<PlayerDetailsSO> playerDetailsList;

    #region Tooltip
    [Tooltip("当前玩家可脚本化对象-用于在场景之间引用当前玩家")]
    #endregion Tooltip
    public CurrentPlayerSO currentPlayer;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion Header
    #region Tooltip
    [Tooltip("填充声音主混音器组")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;

    #region Tooltip
    [Tooltip("门的开与关音效")]
    #endregion Tooltip
    public SoundEffectSO doorOpenCloseSoundEffect;

    #region Header MATERIALS
    [Space(10)]
    [Header("材质")]
    #endregion
    #region Tooltip
    [Tooltip("淡入淡出材质")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("默认精灵光照材质")]
    #endregion
    public Material litMaterial;
    #region Tooltip

    [Tooltip("可变光照着色器")]
    #endregion
    public Shader variableLitShader;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with heart image prefab")]
    #endregion
    public GameObject heartPrefab;
    #region Tooltip
    [Tooltip("Populate with ammo icon prefab")]
    #endregion
    public GameObject ammoIconPrefab;
    #region Tooltip
    [Tooltip("The score prefab")]
    #endregion
    public GameObject scorePrefab;


    #region Validation
#if UNITY_EDITOR
    // Validate the scriptable object details entered
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlip), tableFlip);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpen), chestOpen);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickup), healthPickup);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickup), ammoPickup);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickup), weaponPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        //HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(musicLowSnapshot), musicLowSnapshot);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(musicOffSnapshot), musicOffSnapshot);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(heartPrefab), heartPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(heartIcon), heartIcon);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(bulletIcon), bulletIcon);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(minimapSkullPrefab), minimapSkullPrefab);
    }

#endif
    #endregion
}

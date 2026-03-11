using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region Header MATERIALS
    [Space(10)]
    [Header("材质")]
    #endregion
    #region Tooltip
    [Tooltip("淡入淡出材质")]
    #endregion
    public Material dimmedMaterial;
}

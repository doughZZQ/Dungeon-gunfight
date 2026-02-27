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
}

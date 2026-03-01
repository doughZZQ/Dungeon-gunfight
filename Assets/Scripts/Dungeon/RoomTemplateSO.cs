using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTemplate_", menuName = "Scriptable Objects/Dungeon/Room Template")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;

    #region Room Prefab
    [Space(10)]
    [Header("房间预制件")]
    #endregion

    #region Tooltip
    [Tooltip("房间的游戏对象预制件 (这将包含房间和环境游戏对象的所有瓦片地图)")]
    #endregion

    public GameObject prefab;

    /// <summary>
    /// 如果复制了可脚本化对象并更改了预制件，则用于重新生成guid
    /// </summary>
    [HideInInspector] public GameObject previousPrefab;

    #region Room Configuration
    [Space(10)]
    [Header("房间配置")]
    #endregion

    #region Tooltip
    [Tooltip("房间节点类型为“脚本对象”。" +
        "房间节点类型对应于房间节点图中使用的房间节点。" +
        "走廊是个例外。在房间节点图中，只有一种走廊类型“corridor”。" +
        "对于房间模板，有2种走廊节点类型：CorridorNS(南北走廊) 和 CorridorEW(东西走廊)。")]
    #endregion

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip
    [Tooltip("如果你想象一个完全包围房间瓦片地图的矩形，房间下限表示该矩形的左下角。" +
        "这应该从房间的瓦片地图中确定。" +
        "（使用坐标画笔指针获取左下角的瓦片地图网格位置。" +
        "注意：这是局部瓦片地图位置，而不是世界位置）")]
    #endregion
    public Vector2Int lowerBounds;

    #region Tooltip
    [Tooltip("如果你想象一个完全包围房间瓦片地图的矩形，房间的上限表示该矩形的右上角。" +
        "这应该从房间的瓦片地图中确定。" +
        "（使用坐标画笔指针获取右上角的瓦片地图网格位置。" +
        "注意：这是局部波浪图位置，而不是世界位置）")]
    #endregion
    public Vector2Int upperBounds;

    #region Tooltip
    [Tooltip("一个房间最多应该有四个门 - 每个指南针方向一个。" +
        "这些瓦片应具有一致的5块瓦片开口尺寸，中间的瓦片位置是门口坐标“位置”。")]
    #endregion
    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip
    [Tooltip("房间在tilemap坐标系中的每个可能的生成位置（用于敌人和箱子）都应该添加到此数组中。")]
    #endregion
    public Vector2Int[] spawnPositionArray;

    /// <summary>
    /// 返回房间模板的门口列表
    /// </summary>
    public List<Doorway> GetDoorwayList()
    {
        return doorwayList;
    }


    #region Validation
#if UNITY_EDITOR
    // 验证可编写脚本对象的字段
    private void OnValidate()
    {
        // 如果GUID为空或预制件更改，则设置唯一的GUID
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        // 检查已填充的生成位置
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
#endif
    #endregion
}

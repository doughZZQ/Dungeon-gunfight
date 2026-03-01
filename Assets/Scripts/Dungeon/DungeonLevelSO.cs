using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    #region Basic Level Details
    [Space(10)]
    [Header("基本关卡细节")]
    #endregion
    #region Tooltip
    [Tooltip("The name for the level")]
    #endregion
    public string levelName;

    #region 关卡房间模板
    [Space(10)]
    [Header("关卡房间模板")]
    #endregion
    #region Tooltip
    [Tooltip("用您希望成为该关卡一部分的房间模板填充列表。您需要确保在该关卡的房间节点图中指定的所有房间节点类型都包含房间模板。")]
    #endregion
    public List<RoomTemplateSO> roomTemplateList;

    #region 关卡房间节点图
    [Space(10)]
    [Header("关卡房间节点图")]
    #endregion
    #region Tooltip
    [Tooltip("用应从该关卡中随机选择的房间节点图填充此列表。")]
    #endregion
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }

        // Check to make sure that room templates are specified for all the node types in the specified node graphs.
        // 检查以确保为指定节点图中的所有节点类型指定了房间模板。
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if (roomTemplateSO == null)
            {
                return;
            }

            if (roomTemplateSO.roomNodeType.isCorridorEastOrWest)
            {
                isEWCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isCorridorNorthOrSouth)
            {
                isNSCorridor = true;
            }

            if (roomTemplateSO.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
        }

        if (!isEWCorridor)
        {
            Debug.Log("在 " + this.roomTemplateList.ToString() + " 房间模板中：未指定 东/西 走廊房间类型！");
        }

        if (!isNSCorridor)
        {
            Debug.Log("在 " + this.roomTemplateList.ToString() + " 房间模板中：未指定 南/北 走廊房间类型！");
        }

        if (!isEntrance)
        {
            Debug.Log("在 " + this.roomTemplateList.ToString() + " 房间模板中：未指定 入口 房间类型！");
        }

        foreach (RoomNodeGraphSO roomNodeGraphSO in roomNodeGraphList)
        {
            if (roomNodeGraphSO == null)
            {
                return;
            }

            foreach (RoomNodeSO roomNodeSO in roomNodeGraphSO.roomNodeList)
            {
                if (roomNodeSO == null)
                {
                    continue;
                }

                // Check that a room template has been specified for each roomNode type.
                // 检查是否已为每种房间节点类型指定了房间模板。

                // Corridors and Entrance already checked. 走廊和入口已经检查过了。
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEastOrWest || 
                    roomNodeSO.roomNodeType.isCorridorNorthOrSouth || roomNodeSO.roomNodeType.isCorridor|| roomNodeSO.roomNodeType.isNone)
                {
                    continue;
                }

                bool isRoomNodeTypeFound = false;

                // Loop through all room templates to check that this node type has been specified. 
                // 遍历所有房间模板，检查是否已指定此节点类型。
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                    {
                        continue;
                    }

                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("在 " + roomNodeGraphSO.name.ToString() + " 节点图中：没有为 "+ 
                        roomNodeSO.roomNodeType.name.ToString() + " 房间类型准备房间模板！");
                }
            }
        }
    }

#endif
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon Generator/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    public void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();

        foreach (RoomNodeSO node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }
    }

    /// <summary>
    /// 通过房间节点类型获取房间节点
    /// </summary>
    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
    {
        foreach (RoomNodeSO node in roomNodeList)
        {
            if (node.roomNodeType == roomNodeType)
            {
                return node;
            }
        }
        return null;
    }

    /// <summary>
    /// 通过房间节点ID获取房间节点
    /// </summary>
    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNodeSO))
        {
            return roomNodeSO;
        }

        return null;
    }

    /// <summary>
    /// 获取给定父房间节点的子房间节点
    /// </summary>
    public IEnumerable<RoomNodeSO> GetChildRoomNodes(RoomNodeSO parentRoomNode)
    {
        foreach (string childNodeID in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNode(childNodeID);
        }
    }


    #region Editor Code
    // 以下代码只能在Unity编辑器中运行
#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 originPosition;
    [HideInInspector] public Vector2 linePosition;

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO roomNodeSO, Vector2 position)
    {
        roomNodeToDrawLineFrom = roomNodeSO;
        originPosition = roomNodeSO.rect.center;
        linePosition = position;
        EditorUtility.SetDirty(this);
    }

#endif
    #endregion
}

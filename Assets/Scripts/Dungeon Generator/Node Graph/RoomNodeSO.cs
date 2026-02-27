using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIDList = new List<string>();
    public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;


    #region Editor Code
#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    public bool isLeftClickDragging = false;
    private bool isSelected = false;
    private bool isConnected = false;

    public bool IsSelected { get { return isSelected; } set { isSelected = value; } }

    public void Initialize(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        this.roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle roomNodeStyle)
    {
        if (!roomNodeType.isEntrance && !isConnected)
        {
            //使用 BeginArea 绘制节点框
            GUILayout.BeginArea(rect, roomNodeStyle);

            //Start Region To Detect Popup Selection Changes / 启动区域以检测弹出窗口选择更改
            EditorGUI.BeginChangeCheck();
            //Display a popup using the RoomNodeType name values that can be selected from (default to the currently set roomNodeType)
            //使用可从中选择的 RoomNodeType 名称值显示弹出窗口（默认为当前设置的RoomNodeType）
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);

            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }

            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(rect, roomNodeStyle);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }

            GUILayout.EndArea();
        }
    }

    public void ProcessEvents(Event current)
    {
        switch (current.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(current);
                break;

            case EventType.MouseUp:
                ProcessMouseUpEvent(current);
                break;

            case EventType.MouseDrag:
                ProcessMouseDragEvent(current);
                break;

            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event current)
    {
        if (current.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if (current.button == 1)
        {
            ProcessRightClickDownEvent(current);
        }
    }

    private void ProcessMouseUpEvent(Event current)
    {
        if (current.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessMouseDragEvent(Event current)
    {
        if (current.button == 0)
        {
            ProcessLeftMouseDragEvent(current);
        }
    }

    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        if (isSelected)
        {
            isSelected = false;
        }
        else
        {
            isSelected = true;
        }
    }

    private void ProcessRightClickDownEvent(Event current)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, current.mousePosition);
    }

    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }

    private void ProcessLeftMouseDragEvent(Event current)
    {
        isLeftClickDragging = true;
        DragRoomNode(current.delta);
        GUI.changed = true;
    }

    public void DragRoomNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// 向房间节点添加parentID（如果成功添加节点，则返回true，否则返回false）
    /// </summary>
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }

    /// <summary>
    /// 向房间节点添加childID（如果成功添加节点，则返回true，否则返回false）
    /// </summary>
    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        // 检查子节点是否可以有效地添加到父节点
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;

        //childRoomNodeIDList.Add(childID);
        //return true;
    }

    /// <summary>
    /// 从房间节点中删除childID（如果节点成功被删除，则返回true，否则返回false）
    /// </summary>
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        if (childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 从房间节点中删除parentID（如果节点成功被删除，则返回true，否则返回false）
    /// </summary>
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        if (parentRoomNodeIDList.Contains(parentID))
        {
            parentRoomNodeIDList.Remove(parentID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 检查子节点是否可以有效地添加到父节点，如果可以，则返回true，否则返回false
    /// </summary>
    private bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        // 检查节点图中是否已有连接的boss房间
        foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }

        // 如果子节点的类型为Boss房间，并且节点图中已经有连接的boss房间，则返回false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
        {
            return false;
        }

        // 如果子节点的类型为None，则返回false
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }

        // 如果该节点已经有一个子节点具有此子id，则返回false（不能重复连接）
        if (childRoomNodeIDList.Contains(childID))
        {
            return false;
        }

        // 如果该节点的ID与子节点的ID相同，返回false（不能自我连接）
        if (childID == id)
        {
            return false;
        }

        // 如果此childID已在parentID列表中，则返回false（不能循环连接）
        if (parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }

        // 如果子节点已经拥有父节点，返回false
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count > 0)
        {
            return false;
        }

        // 如果该节点与子节点的类型都是走廊，返回false (不允许走廊和走廊相连接)
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
        {
            return false;
        }

        // 如果该节点与子节点的类型都不是走廊，返回false (不允许房间直接连接，必须用走廊连接房间)
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }

        // 如果要为此节点连接走廊，请检查此节点的子走廊数量是否小于允许的最大子走廊数量
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count >= Settings.maxChildCorridors)
        {
            return false;
        }

        // 如果子节点的类型是入口，则返回false。(入口必须始终是顶层父节点)（入口不能作为子节点）
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }

        // 如果该节点的类型是终点(传送房间)，返回false。(终点(传送房间)必须始终是底层子节点)
        if (roomNodeType.isTransmitRoom)
        {
            return false;
        }

        // 如果将房间添加到走廊，请检查此走廊节点是否尚未添加房间
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckIsConnected()
    {
        return !(parentRoomNodeIDList.Count == 0 && childRoomNodeIDList.Count == 0);
    }

    public void SetIsConnected(bool flag)
    {
        isConnected = flag;
    }

    private string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }
#endif
    #endregion
}

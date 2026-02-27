using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    [MenuItem("Custom Editor/Room Node Graph Editor")]
    public static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 15;

    private const float connectingLineWidth = 4f;
    private const float connectingLineArrowSize = 10f;

    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;

    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private static RoomNodeGraphSO currentRoomNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;

        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
            
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }

    /// <summary>
    /// 如果在检查器中双击房间节点图可脚本化对象资源(RoomNodeGraphSO)，则打开房间节点图编辑器窗口。
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraphSO = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        
        if (roomNodeGraphSO != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraphSO;

            return true;
        }

        return false;
    } 

    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);
            ProcessEvents(Event.current);
            DrawDraggedLine();
            DrawRoomNodeConnections();
            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void ProcessEvents(Event current)
    {
        graphDrag = Vector2.zero;

        // 如果鼠标所在的房间节点为空或当前未被拖动，则获取该节点
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(current.mousePosition);
        }
        // 如果鼠标不在房间节点上，或者我们当前正在从房间节点拖动一条线，则处理房间节点图事件
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(current);
        }
        else
        {
            currentRoomNode.ProcessEvents(current);
        }
    }

    /// <summary>
    /// 处理房间节点图事件
    /// </summary>
    private void ProcessRoomNodeGraphEvents(Event current)
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
        if (current.button == 1)// && currentRoomNodeGraph.roomNodeToDrawLineFrom == null) 
        { 
            ShowContextMenu(current.mousePosition);
        }
        else if (current.button == 0)
        {
            ClearLineDrag();
            ClearRoomNodeSelect();
           
        }
    }

    private void ProcessMouseUpEvent(Event current)
    {
        if (current.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRightMouseUpEvent(current);
        }
    }

    private void ProcessMouseDragEvent(Event current)
    {
        if (current.button == 1)
        {
            ProcessRightMouseDragEvent(current);
        }
        else if (current.button == 0)
        {
            ProcessLeftMouseDragEvent(current);
        }
    }

    private void ProcessRightMouseUpEvent(Event current)
    {
        if (currentRoomNode != null && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO originRoomNode = currentRoomNodeGraph.roomNodeToDrawLineFrom;
            if (currentRoomNode != originRoomNode)
            {
                #region
                //if (!originRoomNode.childRoomNodeIDList.Contains(currentRoomNode.id))
                //{
                //    originRoomNode.childRoomNodeIDList.Add(currentRoomNode.id);
                //}

                //if (!currentRoomNode.parentRoomNodeIDList.Contains(originRoomNode.id))
                //{
                //    currentRoomNode.parentRoomNodeIDList.Add(originRoomNode.id);
                //}
                #endregion
                if (originRoomNode.AddChildRoomNodeIDToRoomNode(currentRoomNode.id))
                {
                    currentRoomNode.AddParentRoomNodeIDToRoomNode(originRoomNode.id);
                    originRoomNode.SetIsConnected(true);
                    currentRoomNode.SetIsConnected(true);
                }
            }
        }

        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        }

        ClearLineDrag();
    }

    private void ProcessRightMouseDragEvent(Event current)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null) 
        {
            UpdateLinePosition(current.delta);
            GUI.changed = true;
        }
    }

    private void ProcessLeftMouseDragEvent(Event current)
    {
        graphDrag = current.delta;

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.DragRoomNode(current.delta);
        }

        GUI.changed = true;
    }

    private void UpdateLinePosition(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("创建新的房间节点"), false, CreateNewRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("选中所有房间节点"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("删除选中房间节点之间的连接"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("删除选中的房间节点"), false, DeleteSelectedRoomNodes);
        menu.ShowAsContext();
    }

    private void ClearRoomNodeSelect()
    {
        if (currentRoomNode == null)
        {
            foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
            {
                if (roomNode.IsSelected)
                {
                    roomNode.IsSelected = false;
                    GUI.changed = true;
                }
            }
        }
    }

    private RoomNodeSO IsMouseOverRoomNode(Vector2 mousePosition)
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.rect.Contains(mousePosition))
            {
                return roomNode;
            }
        }

        return null;
    }

    /// <summary>
    /// 在鼠标位置创建一个房间节点
    /// </summary>
    private void CreateNewRoomNode(object mousePositionObject)
    {
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }

    /// <summary>
    /// 删除选中的房间节点
    /// </summary>
    private void DeleteSelectedRoomNodes()
    {
        Queue<RoomNodeSO> roomNodeDeletionQueue = new Queue<RoomNodeSO>();

        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.IsSelected && !roomNode.roomNodeType.isEntrance && !roomNode.roomNodeType.isTransmitRoom)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);

                foreach (string childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                        childRoomNode.SetIsConnected(childRoomNode.CheckIsConnected());
                    }
                }

                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                        parentRoomNode.SetIsConnected(parentRoomNode.CheckIsConnected());
                    }
                }
            }
        }

        while (roomNodeDeletionQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);

            DestroyImmediate(roomNodeToDelete, true);
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// 删除选中房间节点之间的连接
    /// </summary>
    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.IsSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);

                    if (childRoomNode != null && childRoomNode.IsSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        roomNode.SetIsConnected(roomNode.CheckIsConnected());

                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                        childRoomNode.SetIsConnected(childRoomNode.CheckIsConnected());
                    }
                }
            }
        }

        ClearRoomNodeSelect();
    }

    /// <summary>
    /// 选中所有房间节点
    /// </summary>
    private void SelectAllRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.IsSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// 在鼠标位置创建一个房间节点，重载以传递RoomNodeType
    /// </summary>
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // 创建房间节点可脚本化对象资产
        RoomNodeSO roomNodeSO = ScriptableObject.CreateInstance<RoomNodeSO>();

        // 将房间节点添加到当前房间节点图形的房间节点列表
        currentRoomNodeGraph.roomNodeList.Add(roomNodeSO);

        // 设置房间节点的值
        roomNodeSO.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        // 将房间节点添加到房间节点图可脚本化对象资产数据库
        AssetDatabase.AddObjectToAsset(roomNodeSO, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();

        //刷新
        currentRoomNodeGraph.OnValidate();
    }

    /// <summary>
    /// 在图形窗口中绘制房间节点
    /// </summary>
    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.Draw(roomNode.IsSelected ? roomNodeSelectedStyle : roomNodeStyle);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// 绘制节点到鼠标指针的拖动线
    /// </summary>
    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            // 从节点到鼠标指针位置绘制线
            Handles.DrawBezier(currentRoomNodeGraph.originPosition, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.originPosition, currentRoomNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    /// <summary>
    /// 根据节点之间的父子关系绘制带方向连接线
    /// </summary>
    private void DrawRoomNodeConnections()
    {
        List<RoomNodeSO> roomNodeList_ = currentRoomNodeGraph.roomNodeList;
        foreach (RoomNodeSO roomNode_ in roomNodeList_)
        {
            if (roomNode_.childRoomNodeIDList.Count > 0)
            {
                foreach (string id_ in roomNode_.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(id_))
                    {
                        DrawConnectionLine(roomNode_, currentRoomNodeGraph.roomNodeDictionary[id_]);
                        //Debug.Log("绘制节点之间的父子关系绘制带方向连接线");
                        //GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 在父房间节点和子房间节点之间绘制连接线
    /// </summary>
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPos = parentRoomNode.rect.center;
        Vector2 endPos = childRoomNode.rect.center;

        Vector2 midPos = (startPos + endPos) / 2f;
        Vector2 direction = endPos - startPos;

        Vector2 arrowTailPoint0 = midPos - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint1 = midPos + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        Vector2 arrowHeadPoint = midPos + direction.normalized * connectingLineArrowSize;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint0, arrowHeadPoint, arrowTailPoint0, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);

        Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// 为房间节点图编辑器绘制背景网格
    /// </summary>
    /// <param name="gridSize"></param>
    /// <param name="gridOpacity"></param>
    /// <param name="gridColor"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset,
                new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        for (int i = 0; i < horizontalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * i, 0) + gridOffset,
                new Vector3(position.width + gridSize, gridSize * i, 0f) + gridOffset);
        }

        Handles.color = Color.white;
    }

    /// <summary>
    /// 清除拖动线
    /// </summary>
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }


    private List<RoomNodeSO> GetSelectedRoomNodes()
    {
        List<RoomNodeSO> roomNodes = new List<RoomNodeSO>();
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.IsSelected)
            {
                roomNodes.Add(roomNode);
            }
        }

        return roomNodes;
    }
}

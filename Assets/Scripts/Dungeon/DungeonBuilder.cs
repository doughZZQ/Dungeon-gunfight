using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    protected override void Awake()
    {
        base.Awake();

        // 加载房间节点类型列表
        LoadRoomNodeTypeList();

    }

    private void OnEnable()
    {
        // Set dimmed material to off
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        // Set dimmed material to fully visible
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    /// <summary>
    /// 加载房间节点类型列表
    /// </summary>
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// 生成随机地牢，如果成功生成了地牢，则返回true，如果失败，则返回false
    /// </summary>
    /// <param name="currentDungeonLevel"></param>
    /// <returns></returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;

        // 将可脚本化对象房间模板加载到字典中
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            // 从房间节点图列表中随机选择一个房间节点图
            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Loop until dungeon successfully built or more than max attempts for node graph
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                // 清除地牢房间游戏对象和地牢房间字典
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                // 尝试为选定的房间节点图构建一个随机地牢
                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }
            
            if (dungeonBuildSuccessful)
            {
                // Instantiate Room Gameobjects
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }

    /// <summary>
    /// Instantiate the dungeon room gameobjects from the prefabs
    /// </summary>
    private void InstantiateRoomGameobjects()
    {
        // Iterate through all dungeon rooms.
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // Calculate room position (remember the room instantiatation position needs to be adjusted by the room template lower bounds)
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            // Instantiate room
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Get instantiated room component from instantiated prefab.
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // Initialise The Instantiated Room
            instantiatedRoom.Initialize(roomGameobject);

            // Save gameobject reference.
            room.instantiatedRoom = instantiatedRoom;

            //// Demo code to set rooms as cleared - except for boss
            //if (!room.roomNodeType.isBossRoom)
            //{
            //    room.isClearedOfEnemies = true;
            //}
        }
    }

    /// <summary>
    /// 将房间模板加载到字典中
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary()
    {
        // Clear room template dictionary
        roomTemplateDictionary.Clear();

        // Load room template list into dictionary
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList); // Duplicate: 重复的
            }
        }
    }

    /// <summary>
    /// 从房间节点图列表中随机选择一个房间节点图
    /// </summary>
    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("列表中无房间节点图！");
            return null;
        }
    }

    /// <summary>
    /// 尝试为指定的房间节点图随机构建地牢。
    /// 如果生成了成功的随机布局，则返回true，否则如果出现问题并需要再次尝试，则返回false。
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {

        // Create Open Room Node Queue
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // 从房间节点图将入口节点添加到房间节点队列
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("No Entrance Node");
            return false;  // 表示地牢没有构建成功
        }

        // 从假设没有房间重叠开始
        bool noRoomOverlaps = true;

        // 处理开放房间节点队列
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // 如果所有房间节点都已处理，并且没有房间重叠，则返回true
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// 处理开放房间节点队列中的房间，如果没有房间重叠，则返回true
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {

        // While room nodes in open room node queue & no room overlaps detected.
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps)
        {
            // Get next room node from open room node queue.
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Add child Nodes to queue from room node graph (with links to this parent Room)
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // if the room is the entrance mark as positioned and add to room dictionary
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                // Add room to room dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);
            }

            // else if the room type isn't an entrance
            else
            {
                // Else get parent room for node
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // See if room can be placed without overlaps
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    /// <summary>
    /// 尝试将房间节点放置在地牢中——如果可以放置房间，则返回房间，否则返回null。
    /// </summary>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        // 初始化并假设重叠，除非另有证明。
        bool roomOverlaps = true;

        // 在房间重叠时执行此操作 - 尝试放置在父对象的所有可用门口上，直到房间成功放置而没有重叠。
        while (roomOverlaps)
        {
            // 为父房间随机选择未连接的可用门口
            List<Doorway> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                // If no more doorways to try then overlap failure.
                Debug.Log("aaaaaaaaaa");
                return false; // room overlaps
            }

            Doorway doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // 为房间节点获取与父门方向一致的随机房间模板
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            // 创建一个房间
            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // Place the room - returns true if the room doesn't overlap
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                // If room doesn't overlap then set to false to exit while loop
                roomOverlaps = false;

                // Mark room as positioned
                room.isPositioned = true;

                // Add room to dictionary
                dungeonBuilderRoomDictionary.Add(room.id, room);

            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;  // no room overlaps
    }

    /// <summary>
    /// 放置房间 - 如果房间不重叠，则返回true，否则返回false
    /// </summary>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {

        // 获取与父房间入口相反方向的入口
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

        // 如果父房间入口对面的房间没有相应的入口，请返回
        if (doorway == null)
        {
            // 只需将父门口标记为不可用，这样我们就不会再次尝试连接它
            Debug.Log("bbbbbbbbbb");
            doorwayParent.IsUnavailable = true;

            return false;
        }

        // Calculate 'world' grid parent doorway position 计算“世界”网格父门口位置
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // Calculate adjustment position offset based on room doorway position that we are trying to connect
        // (e.g. if this doorway is west then we need to add (1,0) to the east parent doorway)
        // 根据我们尝试连接的房间门口位置计算调整位置偏移。（例如，如果这个门口是西面的，那么我们需要在东面的父门口加上（1,0））
        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        // Calculate room lower bounds and upper bounds based on positioning to align with parent doorway
        // 根据定位计算房间的下限和上限，以保证与父门口对齐
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // mark doorways as connected & unavailable
            doorwayParent.IsConnected = true;
            doorwayParent.IsUnavailable = true;

            doorway.IsConnected = true;
            doorway.IsUnavailable = true;

            // return true to show rooms have been connected with no overlap
            return true;
        }
        else
        {
            // Just mark the parent doorway as unavailable so we don't try and connect it again
            doorwayParent.IsUnavailable = true;

            return false;
        }
    }



    /// <summary>
    /// 为房间节点获取随机房间模板，同时考虑父门口方向。
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // If room node is a corridor then select random correct Corridor room template based on
        // parent doorway orientation
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNorthOrSouth));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEastOrWest));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        // Else select random room template
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }


        return roomtemplate;
    }

    /// <summary>
    /// 从房间模板列表中获取与房间类型匹配的随机房间模板并返回它。（如果找不到匹配的房间模板，则返回null）
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Loop through room template list
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // Add matching room templates
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // Return null if list is zero
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Select random room template from list and return
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];

    }

    /// <summary>
    /// 获取未连接的入口
    /// </summary>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        // Loop through doorway list
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.IsConnected && !doorway.IsUnavailable)
                yield return doorway;
        }
    }

    /// <summary>
    /// 基于 roomTemplate 和 roomNode 创建房间，并返回创建的房间。
    /// </summary>
    // Create room based on roomTemplate and layoutNode, and return the created room.
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // Initialise room from template
        Room room = new Room();

        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        //room.battleMusic = roomTemplate.battleMusic;
        //room.ambientMusic = roomTemplate.ambientMusic;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        //room.enemiesByLevelList = roomTemplate.enemiesByLevelList;
        //room.roomLevelEnemySpawnParametersList = roomTemplate.roomEnemySpawnParametersList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        // Set parent ID for room
        if (roomNode.parentRoomNodeIDList.Count == 0) // Entrance
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            // Set entrance in game manager
            GameManager.Instance.SetCurrentRoom(room);

        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        //// If there are no enemies to spawn then default the room to be clear of enemies
        //if (room.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        //{
        //    room.isClearedOfEnemies = true;
        //}

        return room;
    }

    /// <summary>
    /// 创建字符串列表的深拷贝
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

    /// <summary>
    /// 创建 入口(Doorway) 列表的深拷贝
    /// </summary>
    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.IsConnected = doorway.IsConnected;
            newDoorway.IsUnavailable = doorway.IsUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }

    /// <summary>
    /// 从门口列表中获取与门口方向相反的门口
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {

        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;
    }

    /// <summary>
    /// 检查是否有房间与被测试房间(roomToTest)的上限和下限参数重叠，如果有重叠的房间，则返回房间，否则返回null
    /// </summary>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // Iterate through all rooms
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // skip if same room as room to test or room hasn't been positioned
            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            // If room overlaps
            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }


        // Return
        return null;

    }

    /// <summary>
    /// 检查两个房间是否相互重叠-如果重叠则返回true，如果不重叠则返回false
    /// </summary>
    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// 检查 间隔1 是否与 间隔2 重叠 - 此方法由IsOverlappingRoom方法使用
    /// </summary>
    // Check if interval 1 overlaps interval 2 - this method is used by the IsOverlappingRoom method.
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 通过房间模板ID获取房间模板，如果ID不存在，则返回null
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 通过房间ID获取房间，如果不存在具有该ID的房间，则返回null
    /// </summary>
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 清除地牢房间游戏对象和地牢房间字典
    /// </summary>
    private void ClearDungeon()
    {
        // 销毁实例化的地牢游戏对象并清除地牢生成器的房间字典
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }
}

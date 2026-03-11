using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    //[HideInInspector] public int[,] aStarMovementPenalty;  // use this 2d array to store movement penalties from the tilemaps to be used in AStar pathfinding
    //[HideInInspector] public int[,] aStarItemObstacles; // use to store position of moveable items that are obstacles
    [HideInInspector] public Bounds roomColliderBounds;
    //[HideInInspector] public List<MoveItem> moveableItemsList = new List<MoveItem>();

    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("对象引用")]

    #endregion Header OBJECT REFERENCES
    #region Tooltip

    [Tooltip("填充环境子占位符游戏对象")]

    #endregion Tooltip
    [SerializeField] private GameObject environmentGameObject;

    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider2D.bounds;
    }

    /// <summary>
    /// 初始化 实例房间
    /// </summary>
    public void Initialize(GameObject roomGameobject)
    {
        PopulateTilemapMemberVariables(roomGameobject);

        BlockOffUnusedDoorWays();

        //AddObstaclesAndPreferredPaths();

        //CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();

    }

    /// <summary>
    /// Populate the tilemap and grid memeber variables.
    /// </summary>
    // 填充tilemap和网格memeber变量。
    private void PopulateTilemapMemberVariables(GameObject roomGameobject)
    {
        // Get the grid component.
        grid = roomGameobject.GetComponentInChildren<Grid>();

        // Get tilemaps in children.
        Tilemap[] tilemaps = roomGameobject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
        }
    }

    /// <summary>
    /// 封闭房间内未使用的入口
    /// </summary>
    private void BlockOffUnusedDoorWays()
    {
        // Loop through all doorways
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.IsConnected)
                continue;

            // 使用瓦片地图上的瓦片来封闭不连通的入口
            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(collisionTilemap, doorway);
            }

            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(minimapTilemap, doorway);
            }

            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(groundTilemap, doorway);
            }

            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration1Tilemap, doorway);
            }

            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapLayer(decoration2Tilemap, doorway);
            }

            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapLayer(frontTilemap, doorway);
            }
        }
    }

    /// <summary>
    /// Block a doorway on a tilemap layer. 在瓦片地图图层上封锁一扇门
    /// </summary>
    private void BlockADoorwayOnTilemapLayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;

            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;

            case Orientation.none:
                break;
        }

    }

    /// <summary>
    /// Block doorway horizontally - for North and South doorways
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + 1 + xPos, startPosition.y - yPos, 0), transformMatrix);
            }
        }
    }

    /// <summary>
    /// Block doorway vertically - for East and West doorways
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;

        // loop through all tiles to copy
        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                // Get rotation of tile being copied
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0));

                // Copy tile
                tilemap.SetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), tilemap.GetTile(new Vector3Int(startPosition.x + xPos, startPosition.y - yPos, 0)));

                // Set rotation of tile copied
                tilemap.SetTransformMatrix(new Vector3Int(startPosition.x + xPos, startPosition.y - 1 - yPos, 0), transformMatrix);

            }
        }
    }

    /// <summary>
    /// Add opening doors if this is not a corridor room
    /// </summary>
    private void AddDoorsToRooms()
    {
        // if the room is a corridor then return
        if (room.roomNodeType.isCorridorEastOrWest || room.roomNodeType.isCorridorNorthOrSouth) return;

        // Instantiate door prefabs at doorway positions
        foreach (Doorway doorway in room.doorwayList)
        {
            // if the doorway prefab isn't null and the doorway is connected
            if (doorway.doorPrefab != null && doorway.IsConnected)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y + tileDistance, 0f);
                }
                else if (doorway.orientation == Orientation.south)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2f, doorway.position.y, 0f);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y - tileDistance * 1.5f, 0f);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    // create door with parent as the room
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y - tileDistance * 1.5f, 0f);
                }

                //// Get door component
                //Door doorComponent = door.GetComponent<Door>();

                //// Set if door is part of a boss room
                //if (room.roomNodeType.isBossRoom)
                //{
                //    doorComponent.isBossRoomDoor = true;

                //    // lock the door to prevent access to the room
                //    doorComponent.LockDoor();

                //    // Instantiate skull icon for minimap by door
                //    GameObject skullIcon = Instantiate(GameResources.Instance.minimapSkullPrefab, gameObject.transform);
                //    skullIcon.transform.localPosition = door.transform.localPosition;
                //}
            }
        }
    }

    /// <summary>
    /// 禁用 collision tilemap 渲染器
    /// </summary>
    private void DisableCollisionTilemapRenderer()
    {
        // Disable collision tilemap renderer
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;

    }
}

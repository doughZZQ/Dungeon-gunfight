using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region DUNGEON LEVELS

    [Space(10)]
    [Header("地牢关卡")]

    #endregion DUNGEON LEVELS

    #region Tooltip

    [Tooltip("填充地牢关卡的可脚本化对象")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("填充初始地牢关卡进行测试，第一关 = 0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;

    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    private void Start()
    {
        this.gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();

        // 测试代码
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }

    private void InstantiatePlayer()
    {
        GameObject playerGO = Instantiate(playerDetails.playerPrefab);
        player = playerGO.GetComponent<Player>();
        player.Initialize(playerDetails);
    }

    /// <summary>
    /// 处理游戏状态
    /// </summary>
    private void HandleGameState()
    {
        switch (this.gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelListIndex);
                this.gameState = GameState.playingLevel;
                break;

            case GameState.playingLevel:

                break;

            case GameState.engagingEnemies:
                break;

            case GameState.bossStage:
                break;

            case GameState.engagingBoss:
                break;

            case GameState.levelCompleted:
                break;

            case GameState.gameWon:
                break;

            case GameState.gameLost:
                break;

            case GameState.gamePaused:
                break;

            case GameState.dungeonOverviewMap:
                break;

            case GameState.restartGame:
                break;

            default:
                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("无法从指定的房间和节点图构建地牢！");
        }

        // Set player roughly mid-room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, 
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // Get nearest spawn point in room nearest to player
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    /// <summary>
    /// 获取玩家当前所在的房间
    /// </summary>
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// 将当前房间设置为玩家所在的房间
    /// </summary>
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    /// <summary>
    /// 获取玩家
    /// </summary>
    public Player GetPlayer()
    {
        return player;
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation
}

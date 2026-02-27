using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon Generator/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("仅标记应在编辑器中可见的 RoomNodeType")]
    #endregion Header
    public bool displayInNodeGraphEditor = true;

    #region Header
    [Header("标记类型为走廊")]
    #endregion Header
    public bool isCorridor;

    #region Header
    [Header("标记类型为南北朝向的走廊")]
    #endregion Header
    public bool isCorridorNorthOrSouth;

    #region Header
    [Header("标记类型为东西朝向的走廊")]
    #endregion Header
    public bool isCorridorEastOrWest;

    #region Header
    [Header("标记类型为入口(入口房间)")]
    #endregion Header
    public bool isEntrance;

    #region Header
    [Header("标记类型为Boss房间")]
    #endregion Header
    public bool isBossRoom;

    #region Header
    [Header("标记类型为终点(传送房间)")]
    #endregion Header
    public bool isTransmitRoom;

    #region Header
    [Header("标记类型为None(未赋值的)")]
    #endregion Header
    public bool isNone;

    #region Validate
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }
#endif
    #endregion
}

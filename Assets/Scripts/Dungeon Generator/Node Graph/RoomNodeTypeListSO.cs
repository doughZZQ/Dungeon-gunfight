using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeList", menuName = "Scriptable Objects/Dungeon Generator/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header Room Node Type List
    [Space(10)]
    [Header("房间节点类型列表")]
    #endregion
    #region Tooltip
    [Tooltip("此列表应填充游戏的所有RoomNodeTypeSO，它被用来代替枚举")]
    #endregion
    public List<RoomNodeTypeSO> list;

    #region Validate
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
#endif
    #endregion
}

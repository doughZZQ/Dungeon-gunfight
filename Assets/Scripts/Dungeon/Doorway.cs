using UnityEngine;

[System.Serializable]
public class Doorway // 入口 类
{
    public Vector2Int position;
    public Orientation Orientation;
    public GameObject doorPrefab;

    #region Header
    [Header("开始复制的左上角位置")]
    #endregion
    public Vector2Int doorwayStartCopyPosition;
    #region Header
    [Header("门口要复制的瓦片宽度")]
    #endregion
    public int doorwayCopyTileWidth;
    #region Header
    [Header("门口要复制的瓦片高度")]
    #endregion
    public int doorwayCopyTileHeight;

    /// <summary>
    /// 该门口是否已连接
    /// </summary>
    public bool IsConnected { get; set; } = false;
    /// <summary>
    /// 该门口是否是不可用的
    /// </summary>
    public bool IsUnavailable { get; set; } = false;
}

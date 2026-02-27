using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region Room Settings

    // 从一个房间引出的子走廊的最大数量。最大值应为3，但不建议这样做，因为这可能会导致地牢构建失败，因为房间更有可能不合群。
    public const int maxChildCorridors = 3;

    #endregion
}

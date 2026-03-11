using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS
    [Space(10)]
    [Header("移动 详细信息")]
    #endregion Header
    #region Tooltip
    [Tooltip("最小移动速度。GetMoveSpeed方法会计算一个介于最小值和最大值之间的随机数值")]
    #endregion Tooltip
    public float minMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("最大移动速度。GetMoveSpeed方法会计算一个介于最小值和最大值之间的随机数值")]
    #endregion Tooltip
    public float maxMoveSpeed = 8f;
    #region Tooltip
    [Tooltip("如果有闪避运动 - 翻滚速度")]
    #endregion
    public float rollSpeed; // for player
    #region Tooltip
    [Tooltip("如果有闪避运动 - 翻滚距离")]
    #endregion
    public float rollDistance; // for player
    #region Tooltip
    [Tooltip("如果有闪避运动 - 这是闪避动作之间的冷却时间（秒）")]
    #endregion
    public float rollCooldownTime; // for player

    /// <summary>
    /// Get a random movement speed between the minimum and maximum values
    /// </summary>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollDistance != 0f || rollSpeed != 0 || rollCooldownTime != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }

    }

#endif
    #endregion Validation
}

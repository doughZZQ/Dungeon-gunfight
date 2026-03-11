using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFENCES
    [Space(10)]
    [Header("对象引用")]
    #endregion
    #region Tooltip
    [Tooltip("在DoorCollider游戏对象上添加BoxCollider2D组件")]
    #endregion
    [SerializeField] private BoxCollider2D doorCollider;

}

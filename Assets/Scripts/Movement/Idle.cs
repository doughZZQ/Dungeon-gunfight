using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidbody();
    }

    /// <summary>
    /// 设置刚体速度（velocity）
    /// </summary>
    private void MoveRigidbody()
    {
        rb2D.velocity = Vector2.zero;
    }
}

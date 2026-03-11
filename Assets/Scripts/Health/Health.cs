using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;

    /// <summary>
    /// 设置初始生命值
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    /// <summary>
    /// 获取初始生命值
    /// </summary>
    public int GetStartingHealth()
    {
        return startingHealth;
    }
}

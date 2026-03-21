using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    #region Header WEAPON SHOOT EFFECT DETAILS
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]
    #endregion Header WEAPON SHOOT EFFECT DETAILS

    #region Tooltip
    [Tooltip("射击效果的颜色渐变。该渐变显示了粒子在生命周期中的颜色变化 - 从左至右。")]
    #endregion Tooltip
    public Gradient colorGradient;

    #region Tooltip
    [Tooltip("粒子系统发射粒子的持续时间。")]
    #endregion Tooltip
    public float duration = 0.50f;

    #region Tooltip
    [Tooltip("粒子特效的起始大小")]
    #endregion Tooltip
    public float startParticleSize = 0.25f;

    #region Tooltip
    [Tooltip("粒子效果的起始速度")]
    #endregion Tooltip
    public float startParticleSpeed = 3f;

    #region Tooltip
    [Tooltip("粒子特效的持续时间")]
    #endregion Tooltip
    public float startLifetime = 0.5f;

    #region Tooltip
    [Tooltip("要发射的最大粒子数")]
    #endregion Tooltip
    public int maxParticleNumber = 100;

    #region Tooltip
    [Tooltip("每秒发射的粒子数。如果为零，则只是突发数字。")]
    #endregion Tooltip
    public int emissionRate = 100;

    #region Tooltip
    [Tooltip("“粒子特效：爆发” 中应该发射多少粒子")]
    #endregion Tooltip
    public int burstParticleNumber = 20;

    #region Tooltip
    [Tooltip("粒子的重力——一个小的负数会使它们浮起来")]
    #endregion
    public float effectGravity = -0.01f;

    #region Tooltip
    [Tooltip("粒子效果的精灵图。如果未指定，则将使用默认粒子子画面")]
    #endregion Tooltip
    public Sprite sprite;

    #region Tooltip
    [Tooltip("粒子在其生命周期内的最小速度。将生成一个介于最小值和最大值之间的随机值。")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMin;

    #region Tooltip
    [Tooltip("粒子在其生命周期内的最大速度。将生成一个介于最小值和最大值之间的随机值。")]
    #endregion Tooltip
    public Vector3 velocityOverLifetimeMax;

    #region Tooltip
    [Tooltip("WeaponShootEffectPrefab 包含射击效果的粒子系统，由 WeaponShootEffectSO 配置")]
    #endregion Tooltip
    public GameObject weaponShootEffectPrefab;


    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);
    }

#endif

    #endregion Validation
}

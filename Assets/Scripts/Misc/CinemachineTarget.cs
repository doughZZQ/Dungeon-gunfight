using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    [SerializeField] private Transform cursorTarget;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    /// <summary>
    /// 设置 cinemachine 摄像机目标组
    /// </summary>
    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineTarget_player = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 4f,
            target = GameManager.Instance.GetPlayer().transform
        };
        
        CinemachineTargetGroup.Target cinemachineTarget_cursor = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 1f,
            target = cursorTarget
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]
        {
            cinemachineTarget_player,
            cinemachineTarget_cursor
        };

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}

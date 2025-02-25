using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    private void SetCinemachineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemachineTargetGroup_player = new CinemachineTargetGroup.Target 
        {
            target = GameManager.Instance.GetPlayer().transform,
            radius = 2.5f,
            weight = 1
        };
        CinemachineTargetGroup.Target cinemachineTargetGroup_cursor = new CinemachineTargetGroup.Target
        {
            target = cursorTarget,
            radius = 1,
            weight = 1
        };
        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]
        {
            cinemachineTargetGroup_player,
            cinemachineTargetGroup_cursor
        };
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }
    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}

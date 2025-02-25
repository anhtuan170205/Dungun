using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetailsSO")]
public class MovementDetailsSO : ScriptableObject
{
    [Header("MOVEMENT DETAILS")]
    public float minMoveSpeed;
    public float maxMoveSpeed;
    public float rollSpeed;
    public float rollDistance;
    public float rollCooldown;

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

    #region VALIDATION
    #if UNITY_EDITOR
    public void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false); 
        if (rollSpeed != 0 || rollDistance != 0 || rollCooldown != 0)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldown), rollCooldown, false);
        }
    }
    #endif
    #endregion
}

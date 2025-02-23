using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform weaponShootPosition;
    [SerializeField] private MovementDetailsSO movementDetails;
    private Player player;
    private float moveSpeed;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetMoveSpeed();
    }
    private void Update()
    {
        MovementInput();
        WeaponInput();
    }
    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector2(horizontalMovement, verticalMovement);
        if (horizontalMovement != 0f && verticalMovement != 0f)
        {
            moveDirection *= 0.7f;
        }
        if (moveDirection != Vector2.zero)
        {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(moveDirection, moveSpeed);
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }
    private void WeaponInput()
    {
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;
        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
        weaponDirection = (mouseWorldPosition - weaponShootPosition.position);
        Vector3 playerDirection = (mouseWorldPosition - transform.position); 
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

    #region VALIDATION
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }
    #endif
    #endregion
}

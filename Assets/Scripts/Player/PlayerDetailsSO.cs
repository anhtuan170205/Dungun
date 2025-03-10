using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/PlayerDetailsSO")]
public class PlayerDetailsSO : ScriptableObject
{
    [Header("PLAYER DETAILS")]
    public string playerCharacterName;
    public GameObject playerPrefab;
    public RuntimeAnimatorController runtimeAnimatorController;
    [Header("PLAYER HEALTH")]
    public int playerHealthAmount;
    [Header("WEAPON")]
    public WeaponDetailsSO startingWeapon;
    public List<WeaponDetailsSO> startingWeaponList;

    [Header("OTHER")]
    public Sprite playerMiniMapIcon;
    public Sprite playerHandSprite;

    #region VALIDATION
    #if UNITY_EDITOR
    private void OnValidate() 
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerPrefab), playerPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSprite), playerHandSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);
    }
    #endif
    #endregion

}

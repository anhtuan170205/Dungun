using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
public class RoomNodeTypeListSO : ScriptableObject
{
    #region Header ROOM NODE TYPE LIST
    [Space(20)]
    [Header("ROOM NODE TYPE LIST")]
    #endregion
    #region Tooltip
    [Tooltip("This list should be populated with all the RoomNodeTypeSO that you want to use in your game - it is used instead of an enum")]
    #endregion
    public List<RoomNodeTypeSO> list;
    #region Validation
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(list), list);
    }
    #endif
    #endregion

}

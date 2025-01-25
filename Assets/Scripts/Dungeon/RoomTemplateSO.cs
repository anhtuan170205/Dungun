using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Room_", menuName = "ScriptableObjects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    [HideInInspector] public string guid;
    #region Header ROOM PREFAB
    [Space(10)]
    [Header("ROOM PREFAB")]
    #endregion Header ROOM PREFAB

    #region Tooltip
    [Tooltip("The gameObject for the room (this will contain all the tilemaps for the room and environment game objects)")]
    #endregion Tooltip
    public GameObject prefab;
    [HideInInspector] public GameObject previousPrefab;

    #region Header ROOM CONFIGURATION
    [Space(10)]
    [Header("ROOM CONFIGURATION")]
    #endregion Header ROOM CONFIGURATION

    #region Tooltip
    [Tooltip("The room node type SO, except for the corridors. In room node graph one type of corridor, but in room template 2 types of corridors")]
    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;
    public Vector2Int lowerBounds;
    public Vector2Int upperBounds;

    [SerializeField] public List<Doorway> doorwayList;

    #region Tooltip
    [Tooltip("Each possible spawn position (use for enemies and chests) for the room in tilemap coordinates should be added to this array")]
    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;

    public List<Doorway> getDoorwayList()
    {
        return doorwayList;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate() 
    {
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this);
        }
        HelperUtilities.ValidateCheckingEnumerableValues(this, nameof(doorwayList), doorwayList);
        HelperUtilities.ValidateCheckingEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }
#endif
    #endregion Validation

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Header("BASIC LEVEL DETAILS")]
    public string levelName;
    [Header("ROOM TEMPLATE FOR LEVEL")]
    public List<RoomTemplateSO> roomTemplates;
    [Header("ROOM NODE GRAPH FOR LEVEL")]
    public List<RoomNodeGraphSO> roomNodeGraphs;

    #region VALIDATION
    #if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplates), roomTemplates)) return;
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphs), roomNodeGraphs)) return;

        bool isNSCorridor = false;
        bool isEWCorridor = false;
        bool isEntrance = false;

        foreach (RoomTemplateSO roomTemplateSO in roomTemplates)
        {
            if (roomTemplateSO == null) return;
            if (roomTemplateSO.roomNodeType.isCorridorEW) isEWCorridor = true;
            if (roomTemplateSO.roomNodeType.isCorridorNS) isNSCorridor = true;
            if (roomTemplateSO.roomNodeType.isEntrance) isEntrance = true;
        }

        if (!isEWCorridor) Debug.Log("No East-West Corridor found in " + this.name.ToString());
        if (!isNSCorridor) Debug.Log("No North-South Corridor found in " + this.name.ToString());
        if (!isEntrance) Debug.Log("No Entrance found in " + this.name.ToString());

        foreach (RoomNodeGraphSO roomNodeGraphSO in roomNodeGraphs)
        {
            if (roomNodeGraphSO == null) continue;
            foreach (RoomNodeSO roomNodeSO in roomNodeGraphSO.roomNodeList)
            {
                if (roomNodeSO == null) return;
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS
                    || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone) continue;
                
                bool isRoomTypeFound = false;
                foreach (RoomTemplateSO roomTemplateSO in roomTemplates)
                {
                    if (roomTemplateSO == null) return;
                    if (roomNodeSO.roomNodeType == roomTemplateSO.roomNodeType)
                    {
                        isRoomTypeFound = true;
                        break;
                    }
                }
                if (!isRoomTypeFound)
                    Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph " + roomNodeGraphSO.name.ToString());
            }
        }
    }
    #endif
    #endregion

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;
    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }
    [Header("DUNGEON")]
    public RoomNodeTypeListSO roomNodeTypeList;

    [Header("PLAYER")]
    public CurrentPlayerSO currentPlayer;
    
    [Header("MATERIALS")]
    public Material dimmedMaterial;
}

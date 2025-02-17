using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null; 
    private RoomNodeTypeListSO roomNodeTypeList;
    private Vector2 graphOffset;
    private Vector2 graphDrag;
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    private const float lineWidth = 3f;
    private const float connectingLineArrowLength = 6f;
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }
    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;

        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }
        return false;
    }
    private void OnGUI() 
    {

        if (currentRoomNodeGraph != null)
        {
            DrawGrid(gridSmall, 0.2f, Color.gray);
            DrawGrid(gridLarge, 0.3f, Color.gray);
            DrawDragLine();
            ProcessEvents(Event.current);
            DrawRoomNodesConnections();
            DrawRoomNodes();
        }
        if (GUI.changed)
        {
            Repaint();
        }
    }   
    private void ProcessEvents(Event currentEvent)
    {
        graphDrag = Vector2.zero;

        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }
        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i=currentRoomNodeGraph.roomNodeList.Count-1; i>=0; i--)
        {
            if (currentRoomNodeGraph.roomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraph.roomNodeList[i];
            }
        }
        return null;
    }
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Node"), false, SelectAllRoomNode);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node"), false, DeleteSelectedRoomNode);
        menu.AddItem(new GUIContent("Deleted Selected Room Node Connections"), false, DeleteSelectedRoomNodeConnections);
        menu.ShowAsContext();

    }
    private void CreateRoomNode(object mousePositionObject)
    {
        // Safely cast the object to Vector2
        if (mousePositionObject is Vector2 mousePosition)
        {
            // If this is the first node, create an entrance node.
            if (currentRoomNodeGraph.roomNodeList.Count == 0)
            {
                CreateRoomNode(mousePosition, roomNodeTypeList.list.Find(x => x.isEntrance));
            }
            else
            {
                // Create a corridor node at the mouse position.
                CreateRoomNode(mousePosition, roomNodeTypeList.list.Find(x => x.isCorridor));

                // Create a generic node slightly offset from the mouse position.
                Vector2 offsetPosition = mousePosition + new Vector2(100f, 100f);
                CreateRoomNode(offsetPosition, roomNodeTypeList.list.Find(x => x.isNone));
            }
        }
        else
        {
            Debug.LogError("Invalid mouse position object. Expected Vector2.");
        }
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialize(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        currentRoomNodeGraph.OnValidate();
    }
    private void DeleteSelectedRoomNode()
    {
        Queue<RoomNodeSO> roomNodeDeleteQueue = new Queue<RoomNodeSO>();
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeleteQueue.Enqueue(roomNode);
                foreach (string childRoomNodeID in roomNode.childrenRoomNodeIDList)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);

                    }
                }
                foreach (string parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    RoomNodeSO parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        while (roomNodeDeleteQueue.Count > 0)
        {
            RoomNodeSO roomNodeToDelete = roomNodeDeleteQueue.Dequeue();
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            DestroyImmediate(roomNodeToDelete, true);
            AssetDatabase.SaveAssets();
        }
    }
    private void DeleteSelectedRoomNodeConnections()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childrenRoomNodeIDList.Count > 0)
            {
                for (int i = roomNode.childrenRoomNodeIDList.Count-1; i>=0; i--)
                {
                    RoomNodeSO childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childrenRoomNodeIDList[i]);
                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        ClearAllSelectedRoomNodes();
    }
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightClickDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent.delta);
        }
    }
    private void ProcessRightClickDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    private void ProcessLeftClickDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;
        for (int i=0; i<currentRoomNodeGraph.roomNodeList.Count; i++)
        {
            currentRoomNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }
        GUI.changed = true;
    }
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);
            if (roomNode != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            ClearLineDrag();
        }
    }
    private void DragLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }
    private void DrawGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        float verticalLineCount = Mathf.CeilToInt(position.width + gridSize) / gridSize;
        float horizontalLineCount = Mathf.CeilToInt(position.height + gridSize) / gridSize;

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
        graphOffset += graphDrag * 0.5f;
        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);
        
        for (int i=0; i<verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height, 0f) + gridOffset);
        }
        for (int j=0; j<horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width, gridSize * j, 0f) + gridOffset);
        }
        Handles.color = Color.white;
    }
    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }
        GUI.changed = true;
    }
    private void DrawDragLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, 
            currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition, Color.white, null, lineWidth);
        }
    }
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }
    private void ClearAllSelectedRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
    private void SelectAllRoomNode()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }
    private void DrawRoomNodesConnections()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childrenRoomNodeIDList.Count > 0)
            {
                foreach (string childRoomNodeID in roomNode.childrenRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPos = parentRoomNode.rect.center;
        Vector2 endPos = childRoomNode.rect.center;
        Vector2 midPos = (endPos + startPos) / 2.0f;
        Vector2 direction = endPos - startPos;
        Vector2 arrowTailPoint1 = midPos + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowLength;
        Vector2 arrowTailPoint2 = midPos - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowLength;
        Vector2 arrowHeadPoint = midPos + direction.normalized * connectingLineArrowLength;
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, lineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, lineWidth);
        Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, lineWidth);
        GUI.changed = true;
    }
    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}

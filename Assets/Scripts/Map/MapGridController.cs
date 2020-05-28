using UnityEngine;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;

public class MapGridController : MonoBehaviour
{
    private PathFinding pathFinding;

    public static MapGridController instance;

    private void Awake()
    {
        instance = this;
    }
    //[SerializeField] private PathfindingDebugStepVisual pathfindingDebugStepVisual;
    //[SerializeField] private PathfindingVisual pathfindingVisual;
    //[SerializeField] private CharacterPathfindingMovementHandler characterPathfinding;
    // Use this for initialization
    void Start()
    {
        int height = 8;
        int width = 18;

        pathFinding = new PathFinding(width, height);
        for (int x = 0; x < pathFinding.GetGrid().GetWidth(); x++)
        {
            PathNode pathNode = pathFinding.GetGrid().GetValue(x, 0);
            pathNode.SetIsWalkable(false);
            pathNode.isChangeable = false;
            pathNode = pathFinding.GetGrid().GetValue(x, height - 1);
            pathNode.SetIsWalkable(false);
            pathNode.isChangeable = false;
        }
        for (int y = 0; y < pathFinding.GetGrid().GetHeight(); y++)
        {
            PathNode pathNode = pathFinding.GetGrid().GetValue(0, y);
            pathNode.SetIsWalkable(false);
            pathNode.isChangeable = false;
            pathNode = pathFinding.GetGrid().GetValue(width - 1, y);
            pathNode.SetIsWalkable(false);
            pathNode.isChangeable = false;
        }
        //Grid<int> grid = new Grid<int>(18, 8, 1f, new Vector3(-9,-4));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathFinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathFinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x - 9, path[i].y -4) * 1f + Vector3.one * .5f, new Vector3(path[i + 1].x - 9, path[i + 1].y -4) * 1f + Vector3.one * .5f, Color.green, 5f);
                }
            }
            //characterPathfinding.SetTargetPosition(mouseWorldPosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathFinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathFinding.GetNode(x, y).SetIsWalkable(!pathFinding.GetNode(x, y).isWalkable);
            pathFinding.GetNode(x, y).isChangeable = false;
        }
    }

    public PathFinding GetPathFindingUtil()
    {
        return pathFinding;
    }
}

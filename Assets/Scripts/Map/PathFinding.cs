using System.Collections.Generic;
using UnityEngine;
using System;
public class PathFinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public PathFinding(int width, int height)
    {
        grid = new Grid<PathNode>(width, height, 1f, new Vector3(-9, -4), (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        //grid.GetXY(startWorldPosition, out int startX, out int startY);
        GetNearestWalkableXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f + grid.originPosition);
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        if (startNode == null || endNode == null) {
            return null;
        }
       
        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.comeFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode curNode = GetLowestFCostNode(openList);
            if(curNode == endNode)
            {
                return CalPath(endNode);
            }
            openList.Remove(curNode);
            closedList.Add(curNode);

            foreach (PathNode neighborNode in GetNeighborList(curNode))
            {
                if (closedList.Contains(neighborNode)) continue;

                if (!neighborNode.isWalkable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = curNode.gCost + CalculateDistanceCost(curNode, neighborNode);

                if(tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.comeFromNode = curNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        return null;

    }


    private void GetNearestWalkableXY(Vector3 worldPosition, out int x, out int y)
    {
        float minDis = float.MaxValue;
        x = Mathf.FloorToInt((worldPosition - grid.originPosition).x / grid.GetCellSize());
        y = Mathf.FloorToInt((worldPosition - grid.originPosition).y / grid.GetCellSize());
        if (grid.GetValue(x, y).isChangeable)
        {
            return;
        }
        List<PathNode> neighborList = GetNeighborList(grid.GetValue(x, y));
        foreach (PathNode neighborNode in neighborList)
        {
            if (neighborNode.isChangeable)
            {
                if (Vector3.Distance(grid.GetWorldPosition(neighborNode.x, neighborNode.y), worldPosition) < minDis)
                {
                    minDis = Vector3.Distance(grid.GetWorldPosition(neighborNode.x, neighborNode.y), worldPosition);
                    x = neighborNode.x;
                    y = neighborNode.y;
                }
               
            }
        }
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }


    public bool isEqualInGridPos(Vector3 a, Vector3 b)
    {
        grid.GetXY(a, out int aX, out int aY);
        grid.GetXY(b, out int bX, out int bY);
        if(aX == bX && aY == bY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetValue(x, y);
    }


    private List<PathNode> CalPath(PathNode end) {
        List<PathNode> path = new List<PathNode>();
        path.Add(end);
        PathNode curNode = end;
        while(curNode.comeFromNode != null)
        {
            path.Add(curNode.comeFromNode);
            curNode = curNode.comeFromNode;
        }
        path.Reverse();
        return path;
    }


    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDis = Mathf.Abs(a.x - b.x);
        int yDis = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDis - yDis);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDis, yDis) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = openList[0];

        for (int i = 0; i < pathNodeList.Count; i++) {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }

        }
        return lowestFCostNode;
    }

}

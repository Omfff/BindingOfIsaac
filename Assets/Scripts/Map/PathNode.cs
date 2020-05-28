using System.Collections;
using System.Collections.Generic;
using System;
public class PathNode
{
    private Grid<PathNode> grid;

    public bool isWalkable;

    public bool isChangeable;

    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode comeFromNode;
    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        this.isWalkable = true;
        this.isChangeable = true;
    }

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }
    public override string ToString()
    {
        if (isWalkable)
        {
            return x + "," + y;
        }
        else
        {
            return isWalkable.ToString();
        }
    }

    public void SetIsWalkable( bool isWalkable)
    {
        if (isChangeable)
        {
            this.isWalkable = isWalkable;
            grid.TriggerGridChanged(x, y);
        }

    }
}

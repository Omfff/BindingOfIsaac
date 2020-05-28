using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{

    public Room room;

    [System.Serializable]
    public struct Grids
    {
        public int columns, rows;

        public float vertivalOffset, horizontalOffset;
    };

    public Grids grid;

    public GameObject gridTile;

    public List<Vector2> avaiablePoints = new List<Vector2>();

    private void Awake()
    {
        room = GetComponentInParent<Room>();
        grid.columns = room.Width -1;//padding
        grid.rows = room.Height -1;
        GenerateGrid();

    }

    public void GenerateGrid()
    {
        grid.vertivalOffset += room.GetComponent<Transform>().localPosition.y;
        grid.horizontalOffset += room.GetComponent<Transform>().localPosition.x;
        //啥意思
        for (int y = 0; y < grid.rows; y++)
        {
            for (int x = 0; x < grid.columns; x++)
            {
                GameObject go = Instantiate(gridTile, transform);
                go.GetComponent<Transform>().position = new Vector2(x - (grid.columns - grid.horizontalOffset), y - (grid.rows - grid.vertivalOffset));
                go.name = "X: " + x + ", Y: " + y;
                avaiablePoints.Add(go.transform.position);
            }
        }

        GetComponentInParent<ObjectRoomSpawner>().InitialiseObjectSpawning();
    }
}

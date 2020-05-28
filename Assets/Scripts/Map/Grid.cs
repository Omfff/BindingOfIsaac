using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid<T> {

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    public Vector3 originPosition;
    private T[,] gridArray;
    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<T>, int, int, T> createGrid) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGrid(this, x, y);
            }
        }

        bool showDebug = true;
        if (showDebug) {
            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 15, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            //前面参数是事件发送者,后面是事件详情标签（传递事件的细节）
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString();
            };
        }
    }

    public void TriggerGridChanged(int x, int y)
    {
         debugTextArray[x, y].text = gridArray[x, y].ToString();
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, T value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetValue(Vector3 worldPosition, T value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public T GetValue(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(T);
        }
    }

    public T GetValue(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }


}

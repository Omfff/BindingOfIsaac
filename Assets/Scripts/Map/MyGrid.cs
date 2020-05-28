//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//public class Grid
//{
//    private int width;

//    private int height;

//    private float cellSize;

//    private int[,] gridArray;

//    private Vector3 originPosition;

//    public const int sortingOrderDefault = 5000;

//    private TextMesh[,] debugTextArray;

//    //public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
//    //public class OnGridValueChangedEventArgs : EventArgs
//    //{
//    //    public int x;
//    //    public int y;
//    //}

//    public Grid(int width, int height, float cellSize, Vector3 originPosition)
//    {
//        this.width = width;
//        this.height = height;
//        this.cellSize = cellSize;
//        this.originPosition = originPosition;

//        gridArray = new int[width, height];

//        debugTextArray = new TextMesh[width, height];

//        for (int x = 0; x < gridArray.GetLength(0); x++)
//        {
//            for (int y = 0; y < gridArray.GetLength(1); y++)
//            {
//                debugTextArray[x,y] = CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 25, Color.white, TextAnchor.MiddleCenter);
//                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
//                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
//            }
//        }

//        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
//        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);


//        //OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) => {
//        //    debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString();
//        //};

//        SetValue(2, 1, 56);
//    }

//    private Vector3 GetWorldPosition(int x, int y)
//    {
//        return new Vector3(x, y) * cellSize + originPosition;
//    }

//    public void SetValue(int x, int y, int value)
//    {
//        if (x >= 0 && y >= 0 && x < width && y < height) { 
//            gridArray[x, y] = value;
//            debugTextArray[x, y].text = value.ToString();
//            //if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
//        }
//    }
//    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
//    {
//        if (color == null) color = Color.white;
//        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
//    }

//    // Create Text in the World
//    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
//    {
//        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
//        Transform transform = gameObject.transform;
//        transform.SetParent(parent, false);
//        transform.localPosition = localPosition;
//        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
//        textMesh.anchor = textAnchor;
//        textMesh.alignment = textAlignment;
//        textMesh.text = text;
//        textMesh.fontSize = fontSize;
//        textMesh.color = color;
//        textMesh.characterSize = 0.2f;
//        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
//        return textMesh;
//    }
//}

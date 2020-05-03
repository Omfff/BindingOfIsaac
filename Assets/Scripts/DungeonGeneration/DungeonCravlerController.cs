using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    top = 0,

    left = 1,

    down = 2,

    right = 3

};

public class DungeonCravlerController : MonoBehaviour
{
    public static List<Vector2Int> positionVisited = new List<Vector2Int>();

    private static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new Dictionary<Direction, Vector2Int>
    {
        {Direction.top, Vector2Int.up },
        {Direction.left, Vector2Int.left },
        {Direction.down, Vector2Int.down },
        {Direction.right, Vector2Int.right }
    };

    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData)
    {
        List<DungeonCravler> dungeonCravlers = new List<DungeonCravler>();

        for(int i = 0; i < dungeonData.numberOfCrawlers; i++)
        {
            dungeonCravlers.Add(new DungeonCravler(Vector2Int.zero));
        }

        int iterations = Random.Range(dungeonData.iterationMin, dungeonData.iterationMax);

        for(int i = 0; i < iterations; i++)
        {
            foreach(DungeonCravler dungeonCravler in dungeonCravlers)
            {
                Vector2Int newPos = dungeonCravler.Move(directionMovementMap);
                positionVisited.Add(newPos);
            }
        }

        return positionVisited;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

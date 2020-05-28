using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class RoomInfo
{
    public string name;

    public int X;

    public int Y;
}

public class RoomController : MonoBehaviour
{

    public static RoomController instance;

    string currentWorldName = "Basement";

    RoomInfo currentLoadRoomData;

    Room currRoom;

    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();

    public List<Room> loadedRooms = new List<Room>();

    bool isLoadingRoom = false;

    bool spawnedBossRoom = false;

    bool updatedRooms = false;

    private void Awake()
    {
        instance = this;
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y))
        {
            return;
        }
        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.X = x;
        newRoomData.Y = y;

        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        string roomName = currentWorldName + info.name;

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        //场景的加载分到每一帧里去，而不是放在一帧里导致卡帧
        while(loadRoom.isDone == false)
        {
            // 每一帧都进行是否加载完成的判断，在这个循环里可以加进度条
            yield return null;
        }
    }
    //用来给room调用
    public void RegisterRoom(Room room)
    {
        if(!DoesRoomExist(currentLoadRoomData.X, currentLoadRoomData.Y))
        {
            room.transform.position = new Vector3(
                currentLoadRoomData.X * room.Width,
                currentLoadRoomData.Y * room.Height,
                0);

            room.X = currentLoadRoomData.X;
            room.Y = currentLoadRoomData.Y;
            room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.X + ", " + room.Y;
            room.transform.parent = transform;

            isLoadingRoom = false;

            if(loadedRooms.Count == 0)
            {
                CameraController.instance.currRoom = room;
            }

            loadedRooms.Add(room);
            //room.RemoveUnconnectedDoors();
        }
        else
        {
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }

    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y) != null;
    }

    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(item => item.X == x && item.Y == y);
    }


    // Start is called before the first frame update
    void Start()
    {
        //LoadRoom("Start", 0, 0);
        ////LoadRoom将room添加到room queue中，
        ////然后在update中取检查queue中是否有需要load的room
        ////若有，则使用coroutine去加载场景
        //LoadRoom("Empty", 1, 0);
        //LoadRoom("Empty", 0, 1);
        //LoadRoom("Empty", -1, 0);
        //LoadRoom("Empty", 0, -1);
   
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if (isLoadingRoom)
        {
            return;
        }

        if(loadRoomQueue.Count == 0)
        {
            if (!spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());
            }else if (spawnedBossRoom && !updatedRooms)
            {
                // 将每个room的门状态更新
                foreach(Room room in loadedRooms)
                {
                    room.RemoveUnconnectedDoors();
                }
                UpdateRooms();
                updatedRooms = true;
            }
            return;
        }

        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;

        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));

    }

    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if(loadRoomQueue.Count == 0)
        {
            Room bossRoom = loadedRooms[loadedRooms.Count - 1];
            Room tempRoom = new Room(bossRoom.X, bossRoom.Y);
            Destroy(bossRoom.gameObject);
            var roomToRemove = loadedRooms.Single(r => r.X == tempRoom.X && r.Y == tempRoom.Y);
            loadedRooms.Remove(roomToRemove);
            LoadRoom("End", tempRoom.X, tempRoom.Y);
        }
    }

    public string GetRandomRoomName()
    {
        string[] possibleRooms = new string[]
        {
            "Empty",
            "Basic1"
        };

        return possibleRooms[Random.Range(0, possibleRooms.Length)];
    }

    public void OnPlayerEnterRoom(Room room)
    {
        CameraController.instance.currRoom = room;

        currRoom = room;

        StartCoroutine(RoomCoroutine());

    }

    public IEnumerator RoomCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        UpdateRooms();
    }

    public void UpdateRooms()
    {
        foreach(Room room in loadedRooms)
        {
            if(currRoom != room)
            {
                Enemy[] enemies = room.GetComponentsInChildren<Enemy>();
                if(enemies != null)
                {
                    foreach(Enemy enemy in enemies)
                    {
                        enemy.notInRoom = true;
                        //Debug.Log("Not in room");
                    }

                    foreach(Door door in room.GetComponentsInChildren<Door>())
                    {
                        door.doorCollider.SetActive(false);
                    }
                }
                else
                {
                    foreach (Door door in room.GetComponentsInChildren<Door>())
                    {
                        door.doorCollider.SetActive(false);
                    }
                }
            }
            else
            {
                Enemy[] enemies = room.GetComponentsInChildren<Enemy>();
                if (enemies.Length > 0)
                {
                    foreach (Enemy enemy in enemies)
                    {
                        enemy.notInRoom = false;
                        Debug.Log("In room"); 
                    }
                    foreach (Door door in room.GetComponentsInChildren<Door>())
                    {
                        door.doorCollider.SetActive(true);
                    }
                }
                else
                {
                    foreach (Door door in room.GetComponentsInChildren<Door>())
                    {
                        door.doorCollider.SetActive(false);
                    }
                }
            }
        }
    }
}

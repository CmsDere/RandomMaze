using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MazeComponent
{
    [Header("미로 생성 정보")]
    [SerializeField] int maxRunwayLength = 4;

    [Header("미로 생성 프리팹")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject stairPrefab;
    

    [Header("함정 생성 정보")]
    [SerializeField] int trapAmount = 20;
    [SerializeField] int stoneTrapRange = 4;

    [Header("함정 생성 프리팹")]
    [SerializeField] GameObject tempContinuePrefab;
    [SerializeField] GameObject tempTrapPrefab;
    [SerializeField] Material redMat;

    bool[,,] visited;

    GameObject[,,] cellObjects; // [x, stage, z]
    GameObject[,,,] wallObjects; // [x, stage, z, direction]  
    GameObject[] stageObjects;
    GameObject[] stairObjects;
    GameObject[,] trapObjects;
    GameObject[,,] continueObjects;

    Dictionary<Vector3Int, int> distanceMap = new Dictionary<Vector3Int, int>();
    List<(Vector3Int start, Vector3Int end, string direction)> runways =
        new List<(Vector3Int start, Vector3Int end, string direction)>();

    void Start()
    {
        stageObjects = new GameObject[stageLength];
        stairObjects = new GameObject[stageLength];
        visited = new bool[mazeWidth, mazeHeight, stageLength];
        cellObjects = new GameObject[mazeWidth, stageLength, mazeHeight];
        wallObjects = new GameObject[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
        continueObjects = new GameObject[mazeWidth, stageLength, mazeHeight];

        GenerateMaze();
    }

    void GenerateMaze()
    {
        InitializeMaze();
        for (int i = 0; i < stageLength; i++)
        {
            DFS(0, 0, i);
            DetermineExit(i);

            //VisualizeRunways(i);
            CreateTrapInfo();
        }       
    }

    void InitializeMaze()
    {  
        for (int i = 0; i < stageLength; i++)
        {
            stageObjects[i] = new GameObject($"Stage {i+1}");
            stageObjects[i].transform.parent = this.transform;
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    CreateCell(x, z, i);
                }
            }
        }
    }

    void DFS(int x, int z, int stage, Vector3Int cell = default(Vector3Int), int distance = 0, int prevLengthX = 0, int prevLengthZ = 0)
    {
        visited[x, z, stage] = true;
        distanceMap[cell] = distance;

        foreach(var dir in ShuffleDirection())
        {
            int newX = x + dir.Item1;
            int newZ = z + dir.Item2;

            int runwayLengthX = (x - newX != 0) ? prevLengthX+1 : 1;
            int runwayLengthZ = (z - newZ != 0) ? prevLengthZ+1 : 1;

            Vector3Int nextCell = new Vector3Int(cell.x + dir.Item1, stage, cell.z + dir.Item2);

            if (IsInRange(newX, newZ) && !visited[newX, newZ, stage]
                && runwayLengthX <= maxRunwayLength && runwayLengthZ <= maxRunwayLength)
            {
                // 벽 제거
                RemoveWall(x, z, newX, newZ, stage);
                
                // 새 위치에서 미로 생성
                DFS(newX, newZ, stage, nextCell, distance + 1, runwayLengthX, runwayLengthZ);
            }
            else if (IsInRange(newX, newZ) && !visited[newX, newZ, stage]
                && runwayLengthX > maxRunwayLength && runwayLengthZ > maxRunwayLength)
            {
                prevLengthX = 0;
                prevLengthZ = 0;
                
                continue;
            }          
        }
    }

    void CreateTrapInfo()
    {
        // Trap 생성부
        FindRunway(wallObjects);
        CalcStoneTrapRunway();
        DebugStoneTrapInfo();
    }

    void FindRunway(GameObject[,,,] wallDatas)
    {
        for (int s = 0; s < stageLength; s++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    if (!visited[x, z, s]) continue;

                    if (wallDatas[x, s, z, (int)DIRECTION.EAST].activeSelf == false)
                    {
                        Vector3Int start = new Vector3Int(x, s, z);
                        Vector3Int end = start;

                        while (end.x + 1 < mazeWidth &&
                            wallDatas[end.x + 1, end.y, end.z, (int)DIRECTION.WEST].activeSelf == false)
                        {
                            end.x++;
                        }

                        if (end != start)
                        {
                            runways.Add((start, end, "Horizontal"));
                        }
                    }

                    if (wallDatas[x, s, z, (int)DIRECTION.NORTH].activeSelf == false)
                    {
                        Vector3Int start = new Vector3Int(x, s, z);
                        Vector3Int end = start;

                        while (end.z + 1 < mazeHeight &&
                            wallDatas[end.x, end.y, end.z + 1, (int)DIRECTION.SOUTH].activeSelf == false)
                        {
                            end.z++;
                        }

                        if (end != start)
                        {
                            runways.Add((start, end, "Vertical"));
                        }
                    }
                }
            }
        }
    }

    void CalcStoneTrapRunway()
    {
        for (int i = 0; i < runways.Count; i++)
        {
            if (runways[i].direction == "Vertical")
            {
                if (runways[i].start.z == 0)
                {
                    if (runways[i].end.z - runways[i].start.z < stoneTrapRange - 2)
                    {
                        runways.RemoveAt(i);
                    }
                }
                else
                {
                    if (runways[i].end.z - runways[i].start.z < stoneTrapRange)
                    {
                        runways.RemoveAt(i);
                    }
                }
            }
            if (runways[i].direction == "Horizontal")
            {
                if (runways[i].start.x == 0)
                {
                    if (runways[i].end.x - runways[i].start.x < stoneTrapRange - 2)
                    {
                        runways.RemoveAt(i);
                    }
                }
                else
                {
                    if (runways[i].end.x - runways[i].start.x < stoneTrapRange)
                    {
                        runways.RemoveAt(i);
                    }
                }
            }
        }
    }

    void DebugStoneTrapInfo()
    {
        foreach (var runway in runways)
        {
            Vector3Int startPos = runway.start;
            Vector3Int endPos = runway.end;

            Debug.Log($"Available Stone Runway, Start: {startPos}, End: {endPos}");
        }
    }

    void DetermineExit(int stage)
    {
        // 가장 먼 거리 찾기
        int maxDistance = distanceMap.Values.Max();
        // 해당 거리에 있는 셀 중 하나를 출구로 선택
        Vector3Int exitCell = distanceMap.FirstOrDefault(x => x.Value == maxDistance).Key;
        // 출구로 지정
        SetAsExit(exitCell, stage);
    }

    void SetAsExit(Vector3Int exitCell, int stage)
    {
        float stairHeight = stairPrefab.transform.lossyScale.y / 2;
        
        if (stage < stageLength - 1)
        {
            stairObjects[stage] = Instantiate
            (
                stairPrefab,
                transform.TransformDirection(cellObjects[exitCell.x, exitCell.y, exitCell.z].transform.position + new Vector3(0, stairHeight, 0)),
                StairDirection(exitCell.x, exitCell.y, exitCell.z)
            );
            stairObjects[stage].name = $"Stage {stage + 1} Stair ({exitCell.x}, {exitCell.z})";
            stairObjects[stage].transform.parent = stageObjects[stage].transform;
        }
        else
        {
            return;
        }
        
    }

    Quaternion StairDirection(int x, int stage, int z)
    {
        Quaternion result = Quaternion.identity;

        for (int i = 0; i < (int)DIRECTION.MAX; i++)
        {
            if (wallObjects[x, stage, z, i].activeSelf == false)
            {
                if (i == 0)
                {
                    result = Quaternion.Euler(0, 180, 0);
                }
                else if (i == 1)
                {
                    result = Quaternion.identity;
                }
                else if (i == 2)
                {
                    result = Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    result = Quaternion.Euler(0, -90, 0);
                }
            }
            else
            {
                continue;
            }
        }

        return result;
    }

    void RemoveWall(int x, int z, int newX, int newZ, int stage)
    {
        if (x == newX && z < newZ)
        {
            wallObjects[x, stage, z, (int)DIRECTION.NORTH].SetActive(false);
            wallObjects[newX, stage, newZ, (int)DIRECTION.SOUTH].SetActive(false);
        }
        else if (x == newX && z > newZ)
        {
            wallObjects[x, stage, z, (int)DIRECTION.SOUTH].SetActive(false);
            wallObjects[newX, stage, newZ, (int)DIRECTION.NORTH].SetActive(false);
        }
        else if (z == newZ && x < newX)
        {
            wallObjects[x, stage, z, (int)DIRECTION.EAST].SetActive(false);
            wallObjects[newX, stage, newZ, (int)DIRECTION.WEST].SetActive(false);
        }
        else if (z == newZ && x > newX)
        {
            wallObjects[x, stage, z, (int)DIRECTION.WEST].SetActive(false);
            wallObjects[newX, stage, newZ, (int)DIRECTION.EAST].SetActive(false);
        }
        else
            return;
    }

    List<(int, int)> ShuffleDirection()
    {
        List<(int, int)> directions = new List<(int, int)>
        {
            (1, 0), (-1, 0), (0, 1), (0, -1)
        };

        return directions.OrderBy(x => Random.Range(0, directions.Count)).ToList();
    }

    void CreateCell(int x, int z, int stage)
    {
        float distance = cellPrefab.transform.lossyScale.x / 2;
        Vector3 wallHeight = new Vector3(0, wallPrefab.transform.lossyScale.y / 2, 0);
        float stageHeigth = stage;

        // 지점 오브젝트 Instantiate
        cellObjects[x, stage, z] = Instantiate
        (
            cellPrefab, 
            transform.TransformDirection(new Vector3(x, stageHeigth, z)), 
            Quaternion.identity
        );
        cellObjects[x, stage, z].name = $"Stage {stage+1} Cell ({x}, {z})";
        cellObjects[x, stage, z].transform.parent = stageObjects[stage].transform;
        cellObjects[x, stage, z].tag = "Cell";

        Vector3 basePosition = transform.TransformDirection(cellObjects[x, stage, z].transform.position);

        // 벽 오브젝트 Instantiate
        wallObjects[x, stage, z, (int)DIRECTION.NORTH] = Instantiate
        (
            wallPrefab,
            basePosition + (Vector3.forward * distance) + wallHeight,
            Quaternion.Euler(0, 90, 0)
        );
        wallObjects[x, stage, z, (int)DIRECTION.SOUTH] = Instantiate
        (
            wallPrefab,
            basePosition + (Vector3.back * distance) + wallHeight,
            Quaternion.Euler(0, 90, 0)
        );
        wallObjects[x, stage, z, (int)DIRECTION.WEST] = Instantiate
        (
            wallPrefab,
            basePosition + (Vector3.left * distance) + wallHeight,
            Quaternion.identity
        );
        wallObjects[x, stage, z, (int)DIRECTION.EAST] = Instantiate
        (
            wallPrefab,
            basePosition + (Vector3.right * distance) + wallHeight,
            Quaternion.identity
        );

        // 벽 오브젝트 이름 지정
        wallObjects[x, stage, z, (int)DIRECTION.NORTH].name = $"Stage {stage+1} North Wall ({x}, {z})";
        wallObjects[x, stage, z, (int)DIRECTION.SOUTH].name = $"Stage {stage+1} South Wall ({x}, {z})";
        wallObjects[x, stage, z, (int)DIRECTION.WEST].name = $"Stage {stage+1} West Wall ({x}, {z})";
        wallObjects[x, stage, z, (int)DIRECTION.EAST].name = $"Stage {stage+1} East Wall ({x}, {z})";

        // 벽 오브젝트 부모 지정
        for (int i = 0; i < (int)DIRECTION.MAX; i++)
        {
            wallObjects[x, stage, z, i].transform.parent = cellObjects[x, stage, z].transform;
            wallObjects[x, stage, z, i].tag = "Wall";
        }      
    }

    public bool[,,] GetVisited()
    {
        return visited;
    }

    public GameObject[,,,] GetWallObjects()
    {
        return wallObjects;
    }
}

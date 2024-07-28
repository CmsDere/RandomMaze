using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MazeComponent
{
    [Header("미로 생성 프리팹")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject stairPrefab;

    bool[,,] visited;

    public GameObject[,,] cellObjects { get; private set; }
    public GameObject[,,,] wallObjects { get; private set; }
    GameObject[] stageObjects;
    GameObject[] stairObjects;

    MazeInformation mazeInfo;

    Dictionary<Vector3Int, int> distanceMap = new Dictionary<Vector3Int, int>();

    void Awake()
    {
        stageObjects = new GameObject[stageLength];
        stairObjects = new GameObject[stageLength];
        visited = new bool[mazeWidth, mazeHeight, stageLength];
        cellObjects = new GameObject[mazeWidth, stageLength, mazeHeight];
        wallObjects = new GameObject[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];

        mazeInfo = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();
    }

    public void GenerateMaze()
    {
        InitializeMaze();
        for (int i = 0; i < stageLength; i++)
        {
            DFS(0, 0, i);
            DetermineExit(i);
            MoveMaze(i);
        }
        GenerateShortcut();
        for (int j = 0; j < stageLength; j++)
        {
            SendMazeInfo(j);
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

    void SendMazeInfo(int stage)
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeHeight; z++)
            {
                if (cellObjects[x, stage, z].activeSelf == true)
                    mazeInfo.cellInfo[x, stage, z] = true;
                else
                    mazeInfo.cellInfo[x, stage, z] = false;

                for (int d = 0; d < (int)DIRECTION.MAX; d++)
                {
                    if (wallObjects[x, stage, z, d].activeSelf == true)
                        mazeInfo.wallInfo[x, stage, z, d] = true;
                    else
                        mazeInfo.wallInfo[x, stage, z, d] = false;
                }
            }
        }

        if (stage < stageLength - 1)
        {
            mazeInfo.stairList.Add((transform.TransformDirection(stairObjects[stage].transform.position), stage));
        }     
        mazeInfo.startPointList.Add((transform.TransformDirection(stageObjects[stage].transform.position), stage));
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

    void MoveMaze(int stage)
    {
        if (stage == 0)
        {
            return;
        }
        else
        {
            stageObjects[stage].transform.position += StairPos(stage) + StairRotToPos(stage);
            stageObjects[stage].transform.rotation = StairRotToStageRot(stage);
        }
    }

    void GenerateShortcut()
    {
        int shortcutCount = Random.Range(10, maxShortcutCount);
        Debug.Log($"지름길 생성 갯수는 {shortcutCount}개입니다.");
        for (int i = 0; i < shortcutCount; i++)
        {
            int randomXCoord = Random.Range(0, mazeWidth);
            int randomZCoord = Random.Range(0, mazeHeight);
            int randomStage = Random.Range(0, stageLength);
            int randomDirection = Random.Range(0, (int)DIRECTION.MAX);

            // 좌표가 0,0일때 서쪽, 남쪽 벽 제거 불가
            if (randomXCoord == 0 && randomZCoord == 0 && (randomDirection == (int)DIRECTION.SOUTH) || randomDirection == (int)DIRECTION.WEST)
            {
                Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : " + generateShortcutError);
            }
            // x좌표가 0일때 서쪽 벽 제거 불가
            else if (randomXCoord == 0 && randomZCoord != 0 && randomDirection == (int)DIRECTION.WEST)
            {
                Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : " + generateShortcutError);
            }
            // x좌표가 19일때 동쪽 벽 제거 불가
            else if (randomXCoord == mazeWidth - 1 && randomZCoord != 0 && randomDirection == (int)DIRECTION.EAST)
            {
                Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : " + generateShortcutError);
            }
            // z좌표가 0일때 남쪽 벽 제거 불가
            else if (randomZCoord == 0 && randomXCoord != 0 && randomDirection == (int)DIRECTION.SOUTH)
            {
                Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : " + generateShortcutError);
            }
            // z좌표가 19일때 북쪽 벽 제거 불가
            else if (randomZCoord == mazeHeight - 1 && randomXCoord != 0 && randomDirection == (int)DIRECTION.NORTH)
            {
                Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : " + generateShortcutError);
            }
            // 그 이외의 나머지 상황에서 벽 제거, 벽이 존재하지 않으면 건너뜀
            else
            {
                if (wallObjects[randomXCoord, randomStage, randomZCoord, randomDirection].activeSelf == false)
                {
                    Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : " + generateShortcutError);
                }
                else
                {
                    // 방향이 북쪽일 때 벽과 맞닿아 있는 바로 다음 지점의 남쪽 벽 제거
                    if (randomDirection == (int)DIRECTION.NORTH)
                    {
                        wallObjects[randomXCoord, randomStage, randomZCoord + 1, (int)DIRECTION.SOUTH].SetActive(false);
                    }
                    // 방향이 남쪽일 때 벽과 맞닿아 있는 바로 다음 지점의 북쪽 벽 제거
                    else if (randomDirection == (int)DIRECTION.SOUTH)
                    {
                        wallObjects[randomXCoord, randomStage, randomZCoord - 1, (int)DIRECTION.NORTH].SetActive(false);
                    }
                    // 방향이 서쪽일 때 벽과 맞닿아 있는 바로 다음 지점의 동쪽 벽 제거
                    else if (randomDirection == (int)DIRECTION.WEST)
                    {
                        wallObjects[randomXCoord - 1, randomStage, randomZCoord, (int)DIRECTION.EAST].SetActive(false);
                    }
                    // 방향이 동쪽일 때 벽과 맞닿아 있는 바로 다음 지점의 서쪽 벽 제거
                    else if (randomDirection == (int)DIRECTION.EAST)
                    {
                        wallObjects[randomXCoord + 1, randomStage, randomZCoord, (int)DIRECTION.WEST].SetActive(false);
                    }
                    wallObjects[randomXCoord, randomStage, randomZCoord, randomDirection].SetActive(false);
                    Debug.Log($"{i} [{randomXCoord}, {randomStage}, {randomZCoord}, {(DIRECTION)randomDirection}] : 지름길 생성 완료");
                }
                
            }
        }
    }

    public Quaternion StairRotToStageRot(int stage)
    {
        Quaternion result = Quaternion.identity;
        Quaternion stair = stairObjects[stage - 1].transform.rotation;

        if (stair == Quaternion.identity) result = Quaternion.identity;
        else if (stair == Quaternion.Euler(0, 180, 0)) result = Quaternion.Euler(0, 180, 0);
        else if (stair == Quaternion.Euler(0, 90, 0)) result = Quaternion.Euler(0, 90, 0);
        else if (stair == Quaternion.Euler(0, -90, 0)) result = Quaternion.Euler(0, -90, 0);

        return result;
    }

    Vector3 StairPos(int stage)
    {
        Vector3 pos = Vector3.zero;
        Vector3 stair = stairObjects[stage - 1].transform.position;

        pos = new Vector3(stair.x, 0, stair.z);

        return pos;
    }

    Vector3 StairRotToPos(int stage)
    {
        Vector3 rot = Vector3.zero;
        Quaternion stair = stairObjects[stage - 1].transform.rotation;

        if (stair == Quaternion.identity) rot = new Vector3(0, 0, 1);
        else if (stair == Quaternion.Euler(0, 180, 0)) rot = new Vector3(0, 0, -1);
        else if (stair == Quaternion.Euler(0, 90, 0)) rot = new Vector3(1, 0, 0);
        else if (stair == Quaternion.Euler(0, -90, 0)) rot = new Vector3(-1, 0, 0);
        else return rot;

        return rot;
    }
}

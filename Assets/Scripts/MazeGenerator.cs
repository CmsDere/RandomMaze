using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("미로 생성 관련 변수")]
    [SerializeField] int stageLength = 3;
    [SerializeField] int maxStraightLength = 4;
    [SerializeField] float finishPointLengthPercent = 0.7f;
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;

    [Header("미로 생성 관련 프리팹")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject stairPrefab;
    
    private bool[,,] visited;
    GameObject[] stages;
    GameObject[] stageFinish;
    int c = 0;

    void Start()
    {
        stages = new GameObject[3];
        stageFinish = new GameObject[3];       

        GenerateMaze();
    }

    void GenerateMaze()
    {
        visited = new bool[width, height,stageLength];

        InitializeMaze();
        for (int i = 0; i < stageLength; i++)
        {
            DFS(0, 0, i);
            SetStartPoint(i);
            SetFinishPoint(i);
            MoveMaze(i);
        }        
    }

    void DFS(int x, int z, int stage, int prevLengthX = 0, int prevLengthZ = 0)
    {
        visited[x, z, stage] = true;

        foreach (var direction in ShuffleDirections())
        {
            int newX = x + direction.Item1;
            int newZ = z + direction.Item2;

            int straightLengthX = (x - newX != 0) ? prevLengthX + 1 : 1;
            int straightLengthZ = (z - newZ != 0) ? prevLengthZ + 1 : 1;

            if (IsInRange(newX, newZ) && !visited[newX, newZ, stage] 
                && straightLengthX <= maxStraightLength && straightLengthZ <= maxStraightLength)
            {
                // 벽 제거
                RemoveWall(x, z, newX, newZ, stage);            

                // 새 위치에서 미로 생성
                DFS(newX, newZ, stage, straightLengthX, straightLengthZ);
            }
            else if (IsInRange(newX, newZ) && !visited[newX, newZ, stage] 
                && straightLengthX > maxStraightLength && straightLengthZ > maxStraightLength)
            {
                prevLengthX = 0;
                prevLengthZ = 0;
                continue;
            }
        }
    }

    List<(int, int)> ShuffleDirections()
    {
        List<(int, int)> directions = new List<(int, int)>
        {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1)
        };

        return directions.OrderBy(x => Random.Range(0, directions.Count)).ToList();
    }

    bool IsInRange(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    // 벽 제거
    // 0번 벽 : 동쪽, 1번 벽 : 서쪽, 2번 벽 : 북쪽, 3번 벽 : 남쪽

    // 동쪽 진행 : z와 newZ가 같고 x가 newX보다 작을 때 x의 동쪽 벽, newX의 서쪽 벽 제거
    // 서쪽 진행 : z와 newZ가 같고 x가 newX보다 클 때 x의 서쪽 벽, newX의 동쪽 벽 제거
    // 북쪽 진행 : x와 newX가 같고 z가 newZ보다 작을 때 z의 북쪽 벽, newZ의 남쪽 벽 제거
    // 남쪽 진행 : x와 newX가 같고 z가 newZ보다 클 때 z의 남쪽 벽, newZ의 북쪽 벽 제거
    void RemoveWall(int x, int z, int newX, int newZ, int stage)
    {
        // 북쪽 진행
        if (x == newX && z < newZ)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} North Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} South Wall {newX} {newZ}"));
        }
        // 남쪽 진행
        else if (x == newX && z > newZ)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} South Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} North Wall {newX} {newZ}"));
        }
        // 동쪽 진행
        else if (z == newZ && x < newX)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} East Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} West Wall {newX} {newZ}"));
        }
        // 서쪽 진행
        else if (z == newZ && x > newX)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} West Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} East Wall {newX} {newZ}"));
        }
        else
        {
            return;
        }
    }

    void InitializeMaze()
    {
        for (int i = 0; i < stageLength; i++)
        {
            stages[i] = new GameObject($"Stage {i+1}");
            stages[i].transform.parent = this.transform;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    CreateCell(x, z, i);
                }
            }
        }       
    }

    void CreateCell(int x, int z, int i)
    {
        float distance = cellPrefab.transform.lossyScale.x / 2;
        Vector3 wallHeight = new Vector3(0, wallPrefab.transform.lossyScale.y / 2, 0);
        float stageHeight = i;

        GameObject cell = Instantiate(cellPrefab, transform.TransformDirection(new Vector3(x, stageHeight, z)), Quaternion.identity);
        cell.name = $"Stage {i + 1} Cell {x} {z}";
        cell.transform.parent = stages[i].transform;     

        // 상하좌우 벽 생성       
        Vector3 basePosition = transform.TransformDirection(cell.transform.position);

        GameObject northWall = Instantiate(wallPrefab, basePosition + Vector3.forward * distance + wallHeight, Quaternion.Euler(0, 90, 0));
        northWall.name = $"Stage {i + 1} North Wall {basePosition.x} {basePosition.z}";
        northWall.transform.parent = cell.transform;
        northWall.tag = "Wall";

        GameObject southWall = Instantiate(wallPrefab, basePosition + Vector3.back * distance + wallHeight, Quaternion.Euler(0, 90, 0));
        southWall.name = $"Stage {i + 1} South Wall {basePosition.x} {basePosition.z}";
        southWall.transform.parent = cell.transform;
        southWall.tag = "Wall";

        GameObject westWall = Instantiate(wallPrefab, basePosition + Vector3.left * distance + wallHeight, Quaternion.identity);
        westWall.name = $"Stage {i + 1} West Wall {basePosition.x} {basePosition.z}";
        westWall.transform.parent = cell.transform;
        westWall.tag = "Wall";

        GameObject eastWall = Instantiate(wallPrefab, basePosition + Vector3.right * distance + wallHeight, Quaternion.identity);
        eastWall.name = $"Stage {i + 1} East Wall {basePosition.x} {basePosition.z}";
        eastWall.transform.parent = cell.transform;
        eastWall.tag = "Wall";

        cell.GetComponent<Cell>().SetCellData(i, c++, basePosition);
        cell.GetComponent<Cell>().SetCellType();
        northWall.GetComponent<Wall>().SetWallData(DIRECTION.NORTH, northWall.transform.localPosition, i);
        southWall.GetComponent<Wall>().SetWallData(DIRECTION.SOUTH, southWall.transform.localPosition, i);
        westWall.GetComponent<Wall>().SetWallData(DIRECTION.WEST, westWall.transform.localPosition, i);
        eastWall.GetComponent<Wall>().SetWallData(DIRECTION.EAST, eastWall.transform.localPosition, i);
    }

    void SetStartPoint(int stage)
    {
        
        if (stage == 0)
        {
            GameObject.Find($"Stage {stage + 1} Cell 0 0").GetComponent<Cell>().SetCellType(CELL_TYPE.START);
            
        }
    }

    void SetFinishPoint(int stage)
    {
        
    }

    /*void SetStartPoint(int stage)
    {
        stageStart[stage] = new GameObject($"Stage {stage + 1} Start Point");
        if (stage == 0)
        {        
            stageStart[stage].transform.position = Vector3.zero;
        }
        else
        {
            stageStart[stage].transform.position = stageFinish[stage-1].transform.position;
        }
        stageStart[stage].transform.parent = stages[stage].transform;
        //stageStart[stage].GetComponent<Cell>().SetCellType(CELL_TYPE.START);
    }

    void SetFinishPoint(int stage)
    {
        int x = Random.Range((int)(width * finishPointLengthPercent), width);
        int z = Random.Range((int)(height * finishPointLengthPercent), height);

        stageFinish[stage] = new GameObject($"Stage {stage + 1} Finish Point");
        stageFinish[stage].transform.position = transform.TransformDirection(new Vector3(x, stage, z));
        stageFinish[stage].transform.parent = stages[stage].transform;
        //stageFinish[stage].GetComponent<Cell>().SetCellType(CELL_TYPE.FINISH);
    }*/

    void MoveMaze(int stage)
    {
        //stages[stage].transform.position = stageStart[stage].transform.position;        
    }

    
}

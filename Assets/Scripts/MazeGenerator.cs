using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int stageLength = 3;
    [SerializeField] int maxStraightLength = 4;
    [SerializeField] float finishPointLengthPercent = 0.7f;

    [SerializeField] int width = 10;
    [SerializeField] int height = 10;
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject wallPrefab;

    

    private bool[,,] visited;
    int[,] cellNum;
    GameObject[] stages;
    GameObject[] stageStart;
    GameObject[] stageFinish;

    int[,] cellNumCs;

    void Start()
    {
        stages = new GameObject[3];
        stageStart = new GameObject[3];
        stageFinish = new GameObject[3];

        

        GenerateMaze();
    }

    void GenerateMaze()
    {
        visited = new bool[width, height,stageLength];
        cellNum = new int[stageLength, width * height - 1];
        cellNumCs = cellNum;
        InitializeMaze();
        for (int i = 0; i < stageLength; i++)
        {
            DFS(0, 0, i);           
        }
        
    }

    void DFS(int x, int z, int stage, int prevLengthX = 0, int prevLengthZ = 0, int cell = 0)
    {
        visited[x, z, stage] = true;
        cellNum[stage, cell] += 1;

        foreach (var direction in ShuffleDirections())
        {
            int newX = x + direction.Item1;
            int newZ = z + direction.Item2;

            int straightLengthX = (x - newX != 0) ? prevLengthX + 1 : 1;
            int straightLengthZ = (z - newZ != 0) ? prevLengthZ + 1 : 1;

            if (IsInRange(newX, newZ) && !visited[newX, newZ, stage] 
                && straightLengthX <= maxStraightLength && straightLengthZ <= maxStraightLength)
            {
                // �� ����
                RemoveWall(x, z, newX, newZ, stage);
                cell++;
                // �� ��ġ���� �̷� ����
                DFS(newX, newZ, stage, straightLengthX, straightLengthZ, cell);                
            }
            else if (IsInRange(newX, newZ) && !visited[newX, newZ, stage] 
                && straightLengthX > maxStraightLength && straightLengthZ > maxStraightLength)
            {
                prevLengthX = 0;
                prevLengthZ = 0;
                continue;
            }
        }

        if (cell > cellNum.GetLength(1))
        {
            cell = 0;
        }

        cellNumCs = cellNum;
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

    // �� ����
    // 0�� �� : ����, 1�� �� : ����, 2�� �� : ����, 3�� �� : ����

    // ���� ���� : z�� newZ�� ���� x�� newX���� ���� �� x�� ���� ��, newX�� ���� �� ����
    // ���� ���� : z�� newZ�� ���� x�� newX���� Ŭ �� x�� ���� ��, newX�� ���� �� ����
    // ���� ���� : x�� newX�� ���� z�� newZ���� ���� �� z�� ���� ��, newZ�� ���� �� ����
    // ���� ���� : x�� newX�� ���� z�� newZ���� Ŭ �� z�� ���� ��, newZ�� ���� �� ����
    void RemoveWall(int x, int z, int newX, int newZ, int stage)
    {
        // ���� ����
        if (x == newX && z < newZ)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} North Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} South Wall {newX} {newZ}"));
        }
        // ���� ����
        else if (x == newX && z > newZ)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} South Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} North Wall {newX} {newZ}"));
        }
        // ���� ����
        else if (z == newZ && x < newX)
        {
            Destroy(GameObject.Find($"Stage {stage + 1} East Wall {x} {z}"));
            Destroy(GameObject.Find($"Stage {stage + 1} West Wall {newX} {newZ}"));
        }
        // ���� ����
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
     
        GameObject cell = Instantiate(cellPrefab, new Vector3(x, stageHeight, z), Quaternion.identity);
        cell.name = $"Stage {i + 1} Cell {x} {z}";
        cell.transform.parent = stages[i].transform;

        // �����¿� �� ����       
        Vector3 basePosition = cell.transform.position;

        GameObject northWall = Instantiate(wallPrefab, basePosition + Vector3.forward * distance + wallHeight, Quaternion.Euler(0, 90, 0));
        northWall.name = $"Stage {i + 1} North Wall {basePosition.x} {basePosition.z}";
        northWall.transform.parent = cell.transform;

        GameObject southWall = Instantiate(wallPrefab, basePosition + Vector3.back * distance + wallHeight, Quaternion.Euler(0, 90, 0));
        southWall.name = $"Stage {i + 1} South Wall {basePosition.x} {basePosition.z}";
        southWall.transform.parent = cell.transform;

        GameObject westWall = Instantiate(wallPrefab, basePosition + Vector3.left * distance + wallHeight, Quaternion.identity);
        westWall.name = $"Stage {i + 1} West Wall {basePosition.x} {basePosition.z}";
        westWall.transform.parent = cell.transform;

        GameObject eastWall = Instantiate(wallPrefab, basePosition + Vector3.right * distance + wallHeight, Quaternion.identity);
        eastWall.name = $"Stage {i + 1} East Wall {basePosition.x} {basePosition.z}";
        eastWall.transform.parent = cell.transform;
    }

    void SetStartPoint(int stage)
    {
        stageStart[stage] = new GameObject($"Stage {stage + 1} Start Point");
        if (stage == 0)
        {        
            stageStart[stage].transform.position = Vector3.zero;
        }
        else
        {
            stageStart[stage].transform.position = stageFinish[stage - 1].transform.position;
        }
    }

    void SetFinishPoint(int stage)
    {
        int finishPoint = Random.Range((int)(cellNum.GetLength(1) / finishPointLengthPercent), cellNum.GetLength(1));
        
        stageFinish[stage] = new GameObject($"Stage {stage + 1} Finish Point");
        for (int i = 0; i < cellNum.GetLength(1); i++)
        {
            if (cellNum[stage, i] == finishPoint)
            {
                //stageFinish[stage].transform.position = 
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DIRECTION
{
    NONE = -1,
    NORTH,
    SOUTH,
    WEST,
    EAST,
    MAX
}

public class MapManager : MonoBehaviour
{
    [Header("미로 생성 정보")]
    public int width = 20;
    public int height = 20;
    public int stageLength = 3;

    bool[,,] visited;
    public bool[,,] cellInfo { get; private set; }
    public bool[,,,] wallInfo { get; private set; }

    void Start()
    {
        visited = new bool[width, stageLength, height];
        cellInfo = new bool[width, stageLength, height];
        wallInfo = new bool[width, stageLength, height, (int)DIRECTION.MAX];
    }

    public void GenerateInfo()
    {
        InitializeInfo();
        for (int y = 0; y < stageLength; y++)
        {
            DFS(0, y, 0);
        }
    }

    void InitializeInfo()
    {
        // cell과 벽 생성
        for (int y = 0; y < stageLength; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    CreateBase(x, y, z);
                }
            }
        }
    }

    /* MapManager에서 미로, 함정, 보물, 지름길, 출구 등 생성하고
     * MapManager에서의 true/false 정보로 MazeGenerator에서 실질적인 오브젝트 생성
     * ex) cellInfo가 true이면 MazeGenerator에서 Instantiate(cell)
     */
    void DFS(int x, int y, int z)
    {
        visited[x, y, z] = true;

        foreach (var dir in ShuffleDirection())
        {
            int newX = x + dir.Item1;
            int newZ = x + dir.Item2;

            if (IsInRange(newX, y, newZ) && !visited[newX, y, newZ])
            {
                // 벽 제거
                RemoveWall(x, z, newX, newZ, y);
                // DFS()
                DFS(newX, y, newZ);
            }
        }
    } 

    void CreateBase(int x, int y, int z)
    {
        cellInfo[x, y, z] = true;
        for(int d = 0; d < (int)DIRECTION.MAX; d++)
        {
            wallInfo[x, y, z, d] = true;
        }
    }

    bool IsInRange(int x, int y, int z)
    {
        return x < width && y < stageLength && z < height && x >= 0 && y >= 0 && z >= 0;
    }

    void RemoveWall(int x, int z, int newX, int newZ, int y)
    {
        if (x == newX && z < newZ)
        {
            wallInfo[x, y, z, (int)DIRECTION.NORTH] = false;
            wallInfo[x, y, z, (int)DIRECTION.SOUTH] = false;
        }
        else if (x == newX && z > newZ)
        {
            wallInfo[x, y, z, (int)DIRECTION.SOUTH] = false;
            wallInfo[x, y, z, (int)DIRECTION.NORTH] = false;
        }
        else if (z == newZ && x < newX)
        {
            wallInfo[x, y, z, (int)DIRECTION.EAST] = false;
            wallInfo[x, y, z, (int)DIRECTION.WEST] = false;
        }
        else if (z == newZ && x > newX)
        {
            wallInfo[x, y, z, (int)DIRECTION.WEST] = false;
            wallInfo[x, y, z, (int)DIRECTION.EAST] = false;
        }
        else
        {
            return;
        }
    }

    List<(int, int)> ShuffleDirection()
    {
        List<(int, int)> list = new List<(int, int)>
        {
            (1, 0), (-1, 0), (0, 1), (0, -1)
        };

        return list.OrderBy(x => Random.Range(0, list.Count)).ToList();
    }
}

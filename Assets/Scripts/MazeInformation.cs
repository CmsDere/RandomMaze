using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MazeInformation : MazeComponent
{
    public bool[,,] cellInfo { get; set; }
    public bool[,,,] wallInfo { get; set; }

    bool[,,] stoneHorizontalVisited;
    bool[,,] stoneVerticalVisited;
    bool[,,] arrowVisited;

    public List<(Vector3 start, Vector3 end, string direction)> runways { get; private set; } 
        = new List<(Vector3 start, Vector3 end, string direction)>();
    public List<(Vector3 pos, string direction)> arrowCell { get; private set; } 
        = new List<(Vector3 pos, string direction)>(); // direction => 화살이 발사되는 방향(N/S 일때 S, W/E 일때 W)
    public List<int> randomArrowCell = new List<int>();
    public Vector3 mazeStartPos;

    void Awake()
    {
        cellInfo = new bool[mazeWidth, stageLength, mazeHeight];
        wallInfo = new bool[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
        stoneHorizontalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        stoneVerticalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        arrowVisited = new bool[mazeWidth, mazeHeight, stageLength];
        mazeStartPos = Vector3.zero;
    }

    public void GenerateStoneTrapInfo()
    {
        for (int i = 0; i < stageLength; i++)
        {
            FindHorizontalRunway(0, 0, i);
            FindVerticalRunway(0, 0, i);
        }
        CalcStoneRunway();
        //DebugStoneRunway();
    }

    public void GenerateArrowTrapInfo()
    {
        // 1. 벽이 2개이상 존재하는 지점 계산
        // 2. 1에서 계산된 지점 중에 무작위로 함정 지점 지정 & 중복 확인
        // 3. 함정이 3개이하로 연속되는지 확인
        // 4. 함정의 위치에 계단이 존재하는지 확인
    }

    // 화살 함정
    void FindCellOfTwoWall(int x, int z, int stage)
    {
        arrowVisited[x, z, stage] = true;

        int newX = (x + 1 != mazeWidth) ? x + 1 : 0;
        int newZ = (newX == 0) ? z + 1 : z;

        if (IsInRange(newX, newZ) && !arrowVisited[newX, newZ, stage])
        {
            if (wallInfo[x, stage, z, (int)DIRECTION.NORTH] == true && wallInfo[x, stage, z, (int)DIRECTION.SOUTH] == true)
            {
                arrowCell.Add((new Vector3(x, stage, z), "South"));
            }
            else if (wallInfo[x, stage, z, (int)DIRECTION.WEST] == true && wallInfo[x, stage, z, (int)DIRECTION.EAST] == true)
            {
                arrowCell.Add((new Vector3(x, stage, z), "WEST"));
            }
            else
            {
                return;
            }
            FindCellOfTwoWall(newX, newZ, stage);
        }
    }

    void CreateArrowTrap()
    {

    }

    void CreateArrowTrapInfo()
    {
        for (int i = 0; i < stageLength * arrowTrapAmount; i++)
        {
            int r = Random.Range(0, arrowCell.Count);
            randomArrowCell.Add(r);
            if (randomArrowCell.Count != randomArrowCell.Distinct().Count())
            {
                randomArrowCell = randomArrowCell.Distinct().ToList();
            }
        }
    }
    //==

    // 돌 함정
    void FindHorizontalRunway(int x, int z, int stage, int distance = 0)
    {
        stoneHorizontalVisited[x, z, stage] = true;

        int newX = (x + 1 != mazeWidth) ? x + 1 : 0;
        int newZ = (newX == 0) ? z + 1 : z;

        if (IsInRange(newX, newZ) && !stoneHorizontalVisited[newX, newZ, stage])
        {
            if (wallInfo[x, stage, z, (int)DIRECTION.EAST] == false && wallInfo[newX, stage, newZ, (int)DIRECTION.WEST] == false)
            {
                FindHorizontalRunway(newX, newZ, stage, distance + 1);
            }
            else
            {
                runways.Add((new Vector3(x - distance, stage, z), new Vector3(x, stage, z), "Horizontal"));
                FindHorizontalRunway(newX, newZ, stage);
            }
        }
    }

    void FindVerticalRunway(int x, int z, int stage, int distance = 0)
    {
        stoneVerticalVisited[x, z, stage] = true;

        int newZ = (z + 1 != mazeHeight) ? z + 1 : 0;
        int newX = (newZ == 0) ? x + 1 : x;

        if (IsInRange(newX, newZ) && !stoneVerticalVisited[newX, newZ, stage])
        {
            if (wallInfo[x, stage, z, (int)DIRECTION.NORTH] == false && wallInfo[newX, stage, newZ, (int)DIRECTION.SOUTH] == false)
            {
                FindVerticalRunway(newX, newZ, stage, distance + 1);
            }
            else
            {
                runways.Add((new Vector3(x, stage, z - distance), new Vector3(x, stage, z), "Vertical"));
                FindVerticalRunway(newX, newZ, stage);
            }
        }
    }

    void CalcStoneRunway()
    {
        for (int i = runways.Count - 1; i >= 0; i--)
        {
            Vector3 start = runways[i].start;
            Vector3 end = runways[i].end;
            if (runways[i].direction == "Horizontal")
            {
                if (end.x - start.x < stoneTrapRange - 1)
                {
                    runways.RemoveAt(i);
                }
            }
            else if (runways[i].direction == "Vertical")
            {
                if (end.z - start.z < stoneTrapRange - 1)
                {
                    runways.RemoveAt(i);
                }
            }  
        }
    }

    void DebugStoneRunway()
    {
        for (int i = 0; i < runways.Count; i++)
        {
            Vector3 start = runways[i].start;
            Vector3 end = runways[i].end;
            string direction = runways[i].direction;

            Debug.Log($"Available Stone Trap {i}, Start: {start}, End: {end}, Direction: {direction}");
        }
    }
    //==
}

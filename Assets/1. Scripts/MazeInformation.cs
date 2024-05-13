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
    bool[,,] flameVisited;

    public List<(Vector3 pos, int stage)> stairList { get; set; } = new List<(Vector3 pos, int stage)>();
    public List<(Vector3 pos, int stage)> startPointList { get; set; } = new List<(Vector3 pos, int stage)>();

    public List<(Vector3 start, Vector3 end, string direction)> runways { get; private set; } 
        = new List<(Vector3 start, Vector3 end, string direction)>();
    public List<(Vector3 pos, string direction)> arrowCell { get; private set; } 
        = new List<(Vector3 pos, string direction)>(); // direction => 화살이 발사되는 방향(N/S 일때 S, W/E 일때 W)
    public List<Vector3> swampCell { get; private set; } = new List<Vector3>();
    public List<Vector3> flameCell { get; private set; } = new List<Vector3>();

    public List<(Vector3 pos, int index)> swampTrapList = new List<(Vector3 pos, int index)>();
    public List<(Vector3 pos, int index)> flameTrapList = new List<(Vector3 pos, int index)>();

    public Vector3 mazeStartPos;

    void Awake()
    {
        cellInfo = new bool[mazeWidth, stageLength, mazeHeight];
        wallInfo = new bool[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
        stoneHorizontalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        stoneVerticalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        arrowVisited = new bool[mazeWidth, mazeHeight, stageLength];
        flameVisited = new bool[mazeWidth, mazeHeight, stageLength];
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
        // 5. 화염 함정과 중복되어 생성되지 않아야함, 돌, 늪 함정과 중복될 수 있음
        for (int i = 0; i < stageLength; i++)
        {
            FindCellOfTwoWall(0, 0, i);
        }
    }

    public void GenerateSwampTrapInfo()
    {
        // 1. 벽 개수에 영향을 받지 않음. 중복 제거
        // 2. 화염 함정과 중복되어 생성되지 않음. 돌, 화살 함정과 중복될 수 있음
        // 3. 계단과 중복되어 생성되지 않음
        // 4. 4개까지 연속하여 생성될 수 있음
        // 5. 리스트 정렬 스테이지 순서로
        FindCellOfWholeWall(swampCell);
        RemoveDuplicateSwampTrap();
        RemoveDuplicateSwampTrapByRule();
        swampTrapList = swampTrapList.OrderBy(x => x.pos.y).ToList();
        Debug.Log(swampTrapList);
        foreach(var s in swampTrapList)
        {
            Debug.Log(s.pos + " " + s.index);
        }
    }

    // 벽 개수에 상관없는 셀 찾기 (늪 함정, 화염 함정)
    void FindCellOfWholeWall(List<Vector3> list)
    {
        for (int y = 0; y < stageLength; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    list.Add(new Vector3(x, y, z));
                }
            }
        }
        Debug.Log("FindCellOfWholeWall");
    }

    // 지정된 개수의 위치를 뽑고 중복된 위치의 지점 제거
    void RemoveDuplicateSwampTrap()
    {
        int i = 0;
        while (i < swampTrapAmount * stageLength)
        {
            int r = Random.Range(0, swampCell.Count);
            swampTrapList.Add((swampCell[r], i));
            if (swampTrapList.Count != (from pos in swampTrapList select pos).Distinct().Count())
            {
                swampTrapList = (from pos in swampTrapList select pos).Distinct().ToList();
                i--;
            }
            else i++;
        }
        foreach (var s in swampTrapList)
        {
            Debug.Log(s.pos + " " + s.index);
        }
        Debug.Log("RemoveDuplicateSwampTrap");
    }
    // 출발지점, 계단지점, 화염함정, 4개 초과 연속 검사
    void RemoveDuplicateSwampTrapByRule()
    {
        // 출발 지점과 중복되지 않음
        swampTrapList = swampTrapList.Where(x => startPointList.Count(s => x.pos == s.pos) == 0).ToList();
        // 계단 지점과 중복되지 않음
        swampTrapList = swampTrapList.Where(x => stairList.Count(s => x.pos == s.pos) == 0).ToList();
        // 화염 함정과 중복되지 않음
        swampTrapList = swampTrapList.Where(x => flameTrapList.Count(s => x.pos == s.pos) == 0).ToList();
        // 4개 초과 연속 검사
        FindHorizontalContinousSwampTrap();
        FindVerticalContinousSwampTrap();
        foreach(var s in swampTrapList)
        {
            Debug.Log(s.pos + " " + s.index);
        }
        Debug.Log("RemoveDuplicateSwampTrapByRule");
        
    }

    void FindHorizontalContinousSwampTrap()
    {
        var hcstQ = from list in swampTrapList where (
                    (list.pos.x == list.pos.x + 1) && (list.pos.x == list.pos.x + 2) && (list.pos.x == list.pos.x + 3)
                    && (list.pos.x == list.pos.x + 4) && (list.pos.x == list.pos.x + 5)
                    ) select list;
        if (swampTrapList.Count != (hcstQ).Distinct().Count())
        {
            swampTrapList = (hcstQ).Distinct().ToList();
        }
        foreach (var s in swampTrapList)
        {
            Debug.Log(s.pos + " " + s.index);
        }
        Debug.Log("FindHorizontalContinousSwampTrap");
    }

    void FindVerticalContinousSwampTrap()
    {
        var vcstQ = from list in swampTrapList
                    where (
                    (list.pos.z == list.pos.z + 1) && (list.pos.z == list.pos.z + 2) && (list.pos.z == list.pos.z + 3)
                    && (list.pos.z == list.pos.z + 4) && (list.pos.z == list.pos.z + 5))
                    select list;
        if (swampTrapList.Count != (vcstQ).Distinct().Count())
        {
            swampTrapList = (vcstQ).Distinct().ToList();
        }
        foreach (var s in swampTrapList)
        {
            Debug.Log(s.pos + " " + s.index);
        }
        Debug.Log("FindVerticalContinousSwampTrap");
    }
    // ==

    public void GenerateFlameTrapInfo()
    {
        // 1. 벽 개수에 영향을 받지 않음. 중복 제거
        // 2. 늪, 화살 함정과 중복될 수 없음. 돌 함정과 중복될 수 있음
        // 3. 계단과 중복되어 생성될 수 없음
        // 4. 2개까지 연속하여 생성될 수 있음.
        for (int i = 0; i < stageLength; i++)
        {
            FindCellOfWholeWallToFlameTrap(0, 0, i);
        }
        
    }

    public void RemoveDuplicatePositionWithOtherTrap()
    {
        swampTrapList = swampTrapList.Where(x => stairList.Count(s => x.pos == s.pos) == 0).ToList();
        flameTrapList = flameTrapList.Where(x => stairList.Count(s => x.pos == s.pos) == 0).ToList();
        flameTrapList = flameTrapList.Where(x => swampTrapList.Count(s => x.pos == s.pos) == 0).ToList();
    }

    // 화염 함정
    void FindCellOfWholeWallToFlameTrap(int x, int z, int stage)
    {
        flameVisited[x, z, stage] = true;

        int newX = (x + 1 != mazeWidth) ? x + 1 : 0;
        int newZ = (newX == 0) ? z + 1 : z;

        if(IsInRange(newX, newZ) && !flameVisited[newX, newZ, stage])
        {
            flameCell.Add(new Vector3(x, stage, z));
            FindCellOfWholeWallToFlameTrap(newX, newZ, stage);
        }
    }

    void RemoveDuplicateFlameTrap()
    {
        for (int i = 0; i < flameCell.Count; i++)
        {
            int r = Random.Range(0, flameCell.Count);
            flameTrapList.Add((flameCell[r], r));
            flameTrapList = (from pos in flameTrapList select pos).Distinct().ToList();
        }
    }
    // ==

    

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

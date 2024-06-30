using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using DefinedTrap;

public class MazeInformation : MazeComponent
{
    public bool[,,] cellInfo { get; set; }
    public bool[,,,] wallInfo { get; set; }

    TrapGenerator tg;

    bool[,,] stoneHorizontalVisited;
    bool[,,] stoneVerticalVisited;
    bool[,,] arrowVisited;

    int wholeSwampAmount;
    int wholeFlameAmount;
    int wholeTreasureAmount;
    public Vector3 mazeStartPos;

    public List<(Vector3 pos, int stage)> stairList { get; set; } = new List<(Vector3 pos, int stage)>();
    public List<(Vector3 pos, int stage)> startPointList { get; set; } = new List<(Vector3 pos, int stage)>();

    public List<(Vector3 start, Vector3 end, string direction)> runways { get; private set; } 
        = new List<(Vector3 start, Vector3 end, string direction)>();
    public List<(Vector3 pos, string direction)> arrowCell { get; private set; } 
        = new List<(Vector3 pos, string direction)>(); // direction => 화살이 발사되는 방향(N/S 일때 S, W/E 일때 W)
    public List<Vector3> swampCell { get; private set; } = new List<Vector3>();
    public List<Vector3> flameCell { get; private set; } = new List<Vector3>();
    public List<Vector3> treasureCell { get; private set; } = new List<Vector3>();

    public List<Vector3> swampTrapList = new List<Vector3>();
    public List<Vector3> flameTrapList = new List<Vector3>();
    public List<Vector3> treasureList = new List<Vector3>();

    void Awake()
    {
        cellInfo = new bool[mazeWidth, stageLength, mazeHeight];
        wallInfo = new bool[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
        stoneHorizontalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        stoneVerticalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        arrowVisited = new bool[mazeWidth, mazeHeight, stageLength];

        tg = GameObject.Find("TrapGenerator").GetComponent<TrapGenerator>();
        
        wholeSwampAmount = stageLength * swampTrapAmount;
        wholeFlameAmount = stageLength * flameTrapAmount;
        wholeTreasureAmount = stageLength * treasureAmount;
        mazeStartPos = Vector3.zero;
    }

    // 지름길 생성


    // 보물 생성
    public void GenerateTreasureInfo()
    {
        FindCellOfWholeWall(treasureCell);
        RemoveDuplicateTrap(wholeTreasureAmount, treasureCell, treasureList);
        RemoveTreasureByRule();
        treasureList = treasureList.OrderBy(x => x.y).ToList();
    }

    void RemoveTreasureByRule()
    {
        // 화살 함정과 중복되지 않음
        treasureList = treasureList.Where(x => tg.arrowTrapList.Count(s => x == s.GetComponent<ArrowTrap>().arrowTrapPos) == 0).ToList();
        // 늪 함정과 중복되지 않음
        treasureList = treasureList.Where(x => swampTrapList.Count(s => x == s) == 0).ToList();
        // 화염 함정과 중복되지 않음
        treasureList = treasureList.Where(x => flameTrapList.Count(s => x == s) == 0).ToList();
        // 시작 지점과 중복되지 않음
        treasureList = treasureList.Where(x => startPointList.Count(s => x == s.pos) == 0).ToList();
        // 계단과 중복되지 않음
        treasureList = treasureList.Where(x => stairList.Count(s => x == s.pos) == 0).ToList();
    }

    // 함정 생성
    public void GenerateStoneTrapInfo()
    {
        for (int i = 0; i < stageLength; i++)
        {
            FindHorizontalRunway(0, 0, i);
            FindVerticalRunway(0, 0, i);
        }
        CalcStoneRunway();
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
        RemoveDuplicateTrap(wholeSwampAmount, swampCell, swampTrapList);
        RemoveDuplicateSwampTrapByRule();
        RemoveOverGenerateTrap(swampTrapList, wholeSwampAmount);
        swampTrapList = swampTrapList.OrderBy(x => x.y).ToList();
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
    }

    // 지정된 개수의 위치를 뽑고 중복된 위치의 지점 제거 (늪 함정, 화염 함정)
    void RemoveDuplicateTrap(int amount, List<Vector3> list1, List<Vector3> list2)
    {
        int i = 0;
        while(i < amount)
        {
            int r = Random.Range(0, list1.Count);
            list2.Add(list1[r]);
            if (list2.Count != list2.Distinct().Count())
            {
                list2 = list2.Distinct().ToList();
                i--;
            }
            else
            {
                i++;
            }
        }
    }
 
    // 출발지점, 계단지점, 화염함정, 4개 초과 연속 검사
    void RemoveDuplicateSwampTrapByRule()
    {
        // 출발 지점과 중복되지 않음
        swampTrapList = swampTrapList.Where(x => startPointList.Count(s => x == s.pos) == 0).ToList();
        // 계단 지점과 중복되지 않음
        swampTrapList = swampTrapList.Where(x => stairList.Count(s => x == s.pos) == 0).ToList();
        // 화염 함정과 중복되지 않음
        swampTrapList = swampTrapList.Where(x => flameTrapList.Count(s => x == s) == 0).ToList();
        // 4개 초과 연속 검사
        
    }
    // ==

    // 총 개수를 넘어서 생성된 함정 제거  
    void RemoveOverGenerateTrap(List<Vector3> list, int amount)
    {
        if (list.Count > amount)
        {
            for (int i = 0; i < list.Count - amount; i++)
            {
                list.RemoveAt(list.Count - i - 1);
            }
        }
    }

    // 화염 함정
    public void GenerateFlameTrapInfo()
    {
        // 1. 벽 개수에 영향을 받지 않음. 중복 제거
        // 2. 늪, 화살 함정과 중복될 수 없음. 돌 함정과 중복될 수 있음
        // 3. 계단과 중복되어 생성될 수 없음
        // 4. 2개까지 연속하여 생성될 수 있음.
        FindCellOfWholeWall(flameCell);
        RemoveDuplicateTrap(wholeFlameAmount, flameCell, flameTrapList);
        RemoveDuplicateFlameTrapByRule();
        RemoveOverGenerateTrap(flameTrapList, wholeFlameAmount);
        flameTrapList = flameTrapList.OrderBy(x => x.y).ToList();
    }

    void RemoveDuplicateFlameTrapByRule()
    {
        // 출발 지점과 중복되지 않음
        flameTrapList = flameTrapList.Where(x => startPointList.Count(s => x == s.pos) == 0).ToList();
        // 계단 지점과 중복되지 않음
        flameTrapList = flameTrapList.Where(x => stairList.Count(s => x == s.pos) == 0).ToList();
        // 화살 함정과 중복되지 않음
        flameTrapList = flameTrapList.Where(x => tg.arrowTrapList.Count(s => x == s.GetComponent<ArrowTrap>().arrowTrapPos) == 0).ToList();
        // 늪 함정과 중복되지 않음
        flameTrapList = flameTrapList.Where(x => swampTrapList.Count(s => x == s) == 0).ToList(); 
        // 2개 초과 연속 검사
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

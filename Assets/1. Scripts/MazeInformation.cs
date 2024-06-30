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
        = new List<(Vector3 pos, string direction)>(); // direction => ȭ���� �߻�Ǵ� ����(N/S �϶� S, W/E �϶� W)
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

    // ������ ����


    // ���� ����
    public void GenerateTreasureInfo()
    {
        FindCellOfWholeWall(treasureCell);
        RemoveDuplicateTrap(wholeTreasureAmount, treasureCell, treasureList);
        RemoveTreasureByRule();
        treasureList = treasureList.OrderBy(x => x.y).ToList();
    }

    void RemoveTreasureByRule()
    {
        // ȭ�� ������ �ߺ����� ����
        treasureList = treasureList.Where(x => tg.arrowTrapList.Count(s => x == s.GetComponent<ArrowTrap>().arrowTrapPos) == 0).ToList();
        // �� ������ �ߺ����� ����
        treasureList = treasureList.Where(x => swampTrapList.Count(s => x == s) == 0).ToList();
        // ȭ�� ������ �ߺ����� ����
        treasureList = treasureList.Where(x => flameTrapList.Count(s => x == s) == 0).ToList();
        // ���� ������ �ߺ����� ����
        treasureList = treasureList.Where(x => startPointList.Count(s => x == s.pos) == 0).ToList();
        // ��ܰ� �ߺ����� ����
        treasureList = treasureList.Where(x => stairList.Count(s => x == s.pos) == 0).ToList();
    }

    // ���� ����
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
        // 1. ���� 2���̻� �����ϴ� ���� ���
        // 2. 1���� ���� ���� �߿� �������� ���� ���� ���� & �ߺ� Ȯ��
        // 3. ������ 3�����Ϸ� ���ӵǴ��� Ȯ��
        // 4. ������ ��ġ�� ����� �����ϴ��� Ȯ��
        // 5. ȭ�� ������ �ߺ��Ǿ� �������� �ʾƾ���, ��, �� ������ �ߺ��� �� ����
        for (int i = 0; i < stageLength; i++)
        {
            FindCellOfTwoWall(0, 0, i);
        }
    }

    public void GenerateSwampTrapInfo()
    {
        // 1. �� ������ ������ ���� ����. �ߺ� ����
        // 2. ȭ�� ������ �ߺ��Ǿ� �������� ����. ��, ȭ�� ������ �ߺ��� �� ����
        // 3. ��ܰ� �ߺ��Ǿ� �������� ����
        // 4. 4������ �����Ͽ� ������ �� ����
        // 5. ����Ʈ ���� �������� ������
        FindCellOfWholeWall(swampCell);
        RemoveDuplicateTrap(wholeSwampAmount, swampCell, swampTrapList);
        RemoveDuplicateSwampTrapByRule();
        RemoveOverGenerateTrap(swampTrapList, wholeSwampAmount);
        swampTrapList = swampTrapList.OrderBy(x => x.y).ToList();
    }

    // �� ������ ������� �� ã�� (�� ����, ȭ�� ����)
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

    // ������ ������ ��ġ�� �̰� �ߺ��� ��ġ�� ���� ���� (�� ����, ȭ�� ����)
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
 
    // �������, �������, ȭ������, 4�� �ʰ� ���� �˻�
    void RemoveDuplicateSwampTrapByRule()
    {
        // ��� ������ �ߺ����� ����
        swampTrapList = swampTrapList.Where(x => startPointList.Count(s => x == s.pos) == 0).ToList();
        // ��� ������ �ߺ����� ����
        swampTrapList = swampTrapList.Where(x => stairList.Count(s => x == s.pos) == 0).ToList();
        // ȭ�� ������ �ߺ����� ����
        swampTrapList = swampTrapList.Where(x => flameTrapList.Count(s => x == s) == 0).ToList();
        // 4�� �ʰ� ���� �˻�
        
    }
    // ==

    // �� ������ �Ѿ ������ ���� ����  
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

    // ȭ�� ����
    public void GenerateFlameTrapInfo()
    {
        // 1. �� ������ ������ ���� ����. �ߺ� ����
        // 2. ��, ȭ�� ������ �ߺ��� �� ����. �� ������ �ߺ��� �� ����
        // 3. ��ܰ� �ߺ��Ǿ� ������ �� ����
        // 4. 2������ �����Ͽ� ������ �� ����.
        FindCellOfWholeWall(flameCell);
        RemoveDuplicateTrap(wholeFlameAmount, flameCell, flameTrapList);
        RemoveDuplicateFlameTrapByRule();
        RemoveOverGenerateTrap(flameTrapList, wholeFlameAmount);
        flameTrapList = flameTrapList.OrderBy(x => x.y).ToList();
    }

    void RemoveDuplicateFlameTrapByRule()
    {
        // ��� ������ �ߺ����� ����
        flameTrapList = flameTrapList.Where(x => startPointList.Count(s => x == s.pos) == 0).ToList();
        // ��� ������ �ߺ����� ����
        flameTrapList = flameTrapList.Where(x => stairList.Count(s => x == s.pos) == 0).ToList();
        // ȭ�� ������ �ߺ����� ����
        flameTrapList = flameTrapList.Where(x => tg.arrowTrapList.Count(s => x == s.GetComponent<ArrowTrap>().arrowTrapPos) == 0).ToList();
        // �� ������ �ߺ����� ����
        flameTrapList = flameTrapList.Where(x => swampTrapList.Count(s => x == s) == 0).ToList(); 
        // 2�� �ʰ� ���� �˻�
    }   
    // ==

    // ȭ�� ����
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

    // �� ����
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

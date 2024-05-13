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
        = new List<(Vector3 pos, string direction)>(); // direction => ȭ���� �߻�Ǵ� ����(N/S �϶� S, W/E �϶� W)
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
        RemoveDuplicateSwampTrap();
        RemoveDuplicateSwampTrapByRule();
        swampTrapList = swampTrapList.OrderBy(x => x.pos.y).ToList();
        Debug.Log(swampTrapList);
        foreach(var s in swampTrapList)
        {
            Debug.Log(s.pos + " " + s.index);
        }
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
        Debug.Log("FindCellOfWholeWall");
    }

    // ������ ������ ��ġ�� �̰� �ߺ��� ��ġ�� ���� ����
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
    // �������, �������, ȭ������, 4�� �ʰ� ���� �˻�
    void RemoveDuplicateSwampTrapByRule()
    {
        // ��� ������ �ߺ����� ����
        swampTrapList = swampTrapList.Where(x => startPointList.Count(s => x.pos == s.pos) == 0).ToList();
        // ��� ������ �ߺ����� ����
        swampTrapList = swampTrapList.Where(x => stairList.Count(s => x.pos == s.pos) == 0).ToList();
        // ȭ�� ������ �ߺ����� ����
        swampTrapList = swampTrapList.Where(x => flameTrapList.Count(s => x.pos == s.pos) == 0).ToList();
        // 4�� �ʰ� ���� �˻�
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
        // 1. �� ������ ������ ���� ����. �ߺ� ����
        // 2. ��, ȭ�� ������ �ߺ��� �� ����. �� ������ �ߺ��� �� ����
        // 3. ��ܰ� �ߺ��Ǿ� ������ �� ����
        // 4. 2������ �����Ͽ� ������ �� ����.
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

    // ȭ�� ����
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

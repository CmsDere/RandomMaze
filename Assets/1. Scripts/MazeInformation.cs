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
    bool[,,] swampVisited;
    bool[,,] flameVisited;

    public List<(Vector3 pos, int stage)> stairList { get; set; } = new List<(Vector3 pos, int stage)>();
    public List<(Vector3 start, Vector3 end, string direction)> runways { get; private set; } 
        = new List<(Vector3 start, Vector3 end, string direction)>();
    public List<(Vector3 pos, string direction)> arrowCell { get; private set; } 
        = new List<(Vector3 pos, string direction)>(); // direction => ȭ���� �߻�Ǵ� ����(N/S �϶� S, W/E �϶� W)
    public List<Vector3> swampCell { get; private set; } = new List<Vector3>();
    public List<Vector3> flameCell { get; private set; } = new List<Vector3>();

    public List<(Vector3 pos, int index)> randomSwampTrapList = new List<(Vector3 pos, int index)>();
    public List<(Vector3 pos, int index)> randomFlameTrapList = new List<(Vector3 pos, int index)>();

    public Vector3 mazeStartPos;

    void Awake()
    {
        cellInfo = new bool[mazeWidth, stageLength, mazeHeight];
        wallInfo = new bool[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
        stoneHorizontalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        stoneVerticalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        arrowVisited = new bool[mazeWidth, mazeHeight, stageLength];
        swampVisited = new bool[mazeWidth, mazeHeight, stageLength];
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
        for (int i = 0; i < stageLength; i++)
        {
            FindCellOfWholeWallToSwampTrap(0, 0, i);
        }
        RemoveDuplicateSwampTrap();
    }

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
        RemoveDuplicateFlameTrap();
    }

    public void RemoveDuplicatePositionWithOtherTrap()
    {
        randomSwampTrapList = randomSwampTrapList.Where(x => stairList.Count(s => x.pos == s.pos) == 0).ToList();
        randomFlameTrapList = randomFlameTrapList.Where(x => stairList.Count(s => x.pos == s.pos) == 0).ToList();
        randomFlameTrapList = randomFlameTrapList.Where(x => randomSwampTrapList.Count(s => x.pos == s.pos) == 0).ToList();
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
            randomFlameTrapList.Add((flameCell[r], r));
            randomFlameTrapList = (from pos in randomFlameTrapList select pos).Distinct().ToList();
        }
    }
    // ==

    // �� ����
    // �� ������ ���� ���� ������ ���� ã��
    void FindCellOfWholeWallToSwampTrap(int x, int z, int stage)
    {
        swampVisited[x, z, stage] = true;

        int newX = (x + 1 != mazeWidth) ? x + 1 : 0;
        int newZ = (newX == 0) ? z + 1 : z;

        if (IsInRange(newX, newZ) && !swampVisited[newX, newZ, stage])
        {
            swampCell.Add(new Vector3(x, stage, z));
            FindCellOfWholeWallToSwampTrap(newX, newZ, stage);
        }
    }
    // �ߺ��� ��ġ�� ���� ����
    void RemoveDuplicateSwampTrap()
    {
        for (int i = 0; i < swampCell.Count; i++)
        {
            int r = Random.Range(0, swampCell.Count);
            randomSwampTrapList.Add((swampCell[r], r));
            randomSwampTrapList = (from pos in randomSwampTrapList select pos).Distinct().ToList();
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

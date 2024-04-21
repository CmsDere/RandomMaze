using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MazeInformation : MazeComponent
{
    public bool[,,] cellInfo { get; set; }
    public bool[,,,] wallInfo { get; set; }

    bool[,,] stoneHorizontalVisited;
    bool[,,] stoneVerticalVisited;

    List<(Vector3Int start, Vector3Int end, string direction)> runways = new List<(Vector3Int start, Vector3Int end, string direction)>();
    

    void Awake()
    {
        cellInfo = new bool[mazeWidth, stageLength, mazeHeight];
        wallInfo = new bool[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
        stoneHorizontalVisited = new bool[mazeWidth, mazeHeight, stageLength];
        stoneVerticalVisited = new bool[mazeWidth, mazeHeight, stageLength];
    }

    public void CreateStoneTrapInfo()
    {
        for (int i = 0; i < stageLength; i++)
        {
            FindHorizontalRunway(0, 0, i);
            //FindVerticalRunway(0, 0, i);
        }
        DebugRunway();
        Debug.Log("==============================");
        CalcStoneRunway();
        Debug.Log("==============================");
        DebugStoneRunway();
        // ------------------------------------
        /*FindRunway();
        CalcStoneTrapRunway();
        DebugStoneRunway();*/
    }

    void FindHorizontalRunway(int x, int z, int stage, int distance = 0)
    {
        stoneHorizontalVisited[x, z, stage] = true;

        int newX = (x + 1 != mazeWidth) ? x + 1 : 0;
        int newZ = (newX == 0) ? z + 1: z;

        if (IsInRange(newX, newZ) && !stoneHorizontalVisited[newX, newZ, stage])
        {

            if (wallInfo[x, stage, z, (int)DIRECTION.EAST] == false && wallInfo[newX, stage, newZ, (int)DIRECTION.WEST] == false)
            {
                FindHorizontalRunway(newX, newZ, stage, distance + 1);
            }
            else
            {
                runways.Add((new Vector3Int(x - distance, stage, z), new Vector3Int(x, stage, z), "Horizontal"));
                FindHorizontalRunway(newX, newZ, stage);
            }
        }
    }

    void FindVerticalRunway(int x, int z, int stage, int distance = 0)
    {
        stoneVerticalVisited[x, z, stage] = true;

        if (IsInRange(x, z+1) && !stoneVerticalVisited[x, z+1, stage])
        {
            if (wallInfo[x, stage, z, (int)DIRECTION.NORTH] == false && wallInfo[x, stage, z+1, (int)DIRECTION.SOUTH] == false)
            {
                FindVerticalRunway((z + 1 < mazeHeight) ? x : x + 1, (z + 1 < mazeHeight) ? z + 1 : 0, stage, distance + 1);
            }
            else
            {
                runways.Add((new Vector3Int(x, stage, z - distance), new Vector3Int(x, stage, z), "Vertical"));
                FindVerticalRunway((z + 1 < mazeHeight) ? x : x + 1, (z + 1 < mazeHeight) ? z + 1 : 0, stage);
            }
        }
    }

    void CalcStoneRunway()
    {
        for (int i = 0; i < runways.Count; i++)
        {
            Vector3Int start = runways[i].start;
            Vector3Int end = runways[i].end;
            if (end.x - start.x < stoneTrapRange - 1)
                Debug.Log(runways[i]);
                runways.RemoveAt(i);
        }
    }

    void FindRunway()
    {
        for (int y = 0; y < stageLength; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    if (!cellInfo[x, y, z]) continue;

                    if (wallInfo[x, y, z, (int)DIRECTION.EAST] == false)
                    {
                        Vector3Int start = new Vector3Int(x, y, z);
                        Vector3Int end = start;

                        while(end.x + 1 < mazeWidth && wallInfo[end.x + 1, end.y, end.z, (int)DIRECTION.WEST] == false)
                        {
                            end.x++;
                        }

                        if (end != start)
                        {
                            runways.Add((start, end, "Horizontal"));
                        }
                    }

                    if (wallInfo[x, y, z, (int)DIRECTION.NORTH] == false)
                    {
                        Vector3Int start = new Vector3Int(x, y, z);
                        Vector3Int end = start;

                        while (end.z + 1 < mazeWidth && wallInfo[end.x, end.y, end.z + 1, (int)DIRECTION.SOUTH] == false)
                        {
                            end.z++;
                        }

                        if (end != start)
                        {
                            runways.Add((start, end, "Vertical"));
                        }
                    }
                }
            }
        }
    }

    void CalcStoneTrapRunway()
    {
        for (int i = 0; i < runways.Count; i++)
        {
            Vector3Int start = runways[i].start;
            Vector3Int end = runways[i].end;
            string direction = runways[i].direction;

            // Vertical
            if (direction == "Vertical")
            {
                if (end.z - start.z < stoneTrapRange - 1)
                    runways.RemoveAt(i);
                /*if (start.z == 0)
                {
                    if (end.z - start.z < stoneTrapRange - 1)
                        runways.RemoveAt(i);
                }
                else
                {
                    if (end.z - start.z < stoneTrapRange - 2)
                        runways.RemoveAt(i);
                }*/
            }
            else if (direction == "Horizontal")
            {
                if (end.x - start.x < stoneTrapRange - 1)
                    runways.RemoveAt(i);
                /*if (start.x == 0)
                {
                    if (end.x - start.x < stoneTrapRange - 1)
                        runways.RemoveAt(i);
                }
                else
                {
                    if (end.x - start.x < stoneTrapRange - 2)
                        runways.RemoveAt(i);
                }*/
            }
        }
    }

    void DebugRunway()
    {
        for (int i = 0; i < runways.Count; i++)
        {
            Debug.Log(runways[i]);
        }
    }

    void DebugStoneRunway()
    {
        for (int i = 0; i < runways.Count; i++)
        {
            Vector3Int start = runways[i].start;
            Vector3Int end = runways[i].end;
            string direction = runways[i].direction;

            Debug.Log($"Available Stone Trap {i}, Start: {start}, End: {end}, Direction: {direction}");
        }
    }
}

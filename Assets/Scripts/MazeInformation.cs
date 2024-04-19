using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MazeInformation : MazeComponent
{
    public bool[,,] cellInfo { get; set; }
    public bool[,,,] wallInfo { get; set; }

    void Start()
    {
        cellInfo = new bool[mazeWidth, stageLength, mazeHeight];
        wallInfo = new bool[mazeWidth, stageLength, mazeHeight, (int)DIRECTION.MAX];
    }

    public void DebugInfo()
    {
        for (int y = 0; y < stageLength; y++)
        {
            for (int x = 0;  x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    for (int d = 0; d < (int)DIRECTION.MAX; d++)
                    {
                        Debug.Log($"WallInfo[{x}, {y}, {z}, {(DIRECTION)d}] = {wallInfo[x, y, z, d]}");
                    }
                }
            }
        }
    }
}

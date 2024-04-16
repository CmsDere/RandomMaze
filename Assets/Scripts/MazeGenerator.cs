using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject wallPrefab;

    GameObject mapManager;
    int width;
    int heigth;
    int stageLength;
    bool[,,] cellInfo;
    bool[,,,] wallInfo;

    GameObject[,,] cellObjects;
    GameObject[,,,] wallObjects;

    void Start()
    {
        cellObjects = new GameObject[width, stageLength, heigth];
        wallObjects = new GameObject[width, stageLength, heigth, (int)DIRECTION.MAX];

        mapManager = GameObject.Find("MapManager");
        width = mapManager.GetComponent<MapManager>().width;
        heigth = mapManager.GetComponent<MapManager>().height;
        stageLength = mapManager.GetComponent<MapManager>().stageLength;
        cellInfo = mapManager.GetComponent<MapManager>().cellInfo;
        wallInfo = mapManager.GetComponent<MapManager>().wallInfo;
    }

    public void GenerateMaze()
    {
        for (int y = 0; y < stageLength; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < heigth; z++)
                {
                    CreateCell(x, y, z);
                }
            }
        }
    }

    void CreateCell(int x, int y, int z)
    {
        float distance = cellPrefab.transform.lossyScale.x / 2;
        Vector3 wallHeight = new Vector3(0, wallPrefab.transform.lossyScale.y / 2, 0);

        if (!cellInfo[x, y, z])
        {
            return;
        }
        else
        {
            // 瘤痢 积己
            cellObjects[x, y, z] = Instantiate
            (
                cellPrefab,
                transform.TransformDirection(new Vector3(x, y, z)),
                Quaternion.identity,
                transform
            );  
        }

        Vector3 basePos = transform.TransformDirection(cellObjects[x, y, z].transform.position);

        for (int d = 0; d < (int)DIRECTION.MAX; d++)
        {
            if (!wallInfo[x, y, z, d])
            {
                return;
            }
        }
        // 合率 寒 积己
        wallObjects[x, y, z, (int)DIRECTION.NORTH] = Instantiate
            (
                wallPrefab,
                basePos + (Vector3.forward * distance) + wallHeight,
                Quaternion.Euler(0, 90, 0),
                cellObjects[x, y, z].transform
            );
        wallObjects[x, y, z, (int)DIRECTION.NORTH].name = $"Stage {y+1} North Wall ({x}, {z})";
        // 巢率 寒 积己
        wallObjects[x, y, z, (int)DIRECTION.SOUTH] = Instantiate
            (
                wallPrefab,
                basePos + (Vector3.back * distance) + wallHeight,
                Quaternion.Euler(0, 90, 0),
                cellObjects[x, y, z].transform
            );
        wallObjects[x, y, z, (int)DIRECTION.SOUTH].name = $"Stage {y + 1} South Wall ({x}, {z})";
        // 辑率 寒 积己
        wallObjects[x, y, z, (int)DIRECTION.WEST] = Instantiate
            (
                wallPrefab,
                basePos + (Vector3.forward * distance) + wallHeight,
                Quaternion.identity,
                cellObjects[x, y, z].transform
            );
        wallObjects[x, y, z, (int)DIRECTION.WEST].name = $"Stage {y + 1} West Wall ({x}, {z})";
        // 悼率 寒 积己
        wallObjects[x, y, z, (int)DIRECTION.EAST] = Instantiate
            (
                wallPrefab,
                basePos + (Vector3.forward * distance) + wallHeight,
                Quaternion.identity,
                cellObjects[x, y, z].transform
            );
        wallObjects[x, y, z, (int)DIRECTION.EAST].name = $"Stage {y + 1} East Wall ({x}, {z})";
    }
}

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

    void GenerateMaze()
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
            // 寒 积己
            
        }
    }
}

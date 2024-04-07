using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    [Header("함정 생성 정보")]
    [SerializeField] int trapAmount = 20;

    [Header("함정 생성 프리팹")]
    [SerializeField] GameObject tempTrapPrefab;

    GameObject[,,] cellObjects; // [x, stage, z]
    GameObject[,,,] wallObjects; // [x, stage, z, wallDirection]
    GameObject[,] trapObjects; // [trapNum, stage]

    void Start()
    {
        cellObjects = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>().cellObjects;
        wallObjects = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>().wallObjects;
        trapObjects = new GameObject[trapAmount, stageLength];

        
    }

    public void GenerateTrap()
    {
        
    }

    

    TRAP_TYPE DetermineTrapType(int x, int z, int stage)
    {
        // north wall과 south wall이 없고 
        if (wallObjects[x, z, stage, (int)DIRECTION.NORTH].activeSelf == false &&
            wallObjects[x, z, stage, (int)DIRECTION.SOUTH].activeSelf == false)
        {
            
        }

        return TRAP_TYPE.NONE;
    }
}

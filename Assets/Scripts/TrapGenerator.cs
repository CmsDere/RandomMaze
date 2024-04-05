using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    [Header("함정 생성 정보")]
    [SerializeField] int trapAmount = 20;

    GameObject[,,] cellObjects; // [x, stage, z]
    GameObject[,,,] wallObjects; // [x, stage, z, wallDirection]
    bool[,,] continuous;

    void Start()
    {
        cellObjects = GetComponent<MazeGenerator>().cellObjects;
        wallObjects = GetComponent<MazeGenerator>().wallObjects;
        continuous = new bool[mazeWidth, mazeHeight, stageLength];
    }

    void GenerateTrap()
    {

    }

    void InitializeTrap()
    {
        for (int i = 0; i < stageLength; i++)
        {
            for (int x  = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    CreateTrap(x, z, i);
                }
            }
        }
    }

    void CreateTrap(int x, int z, int stage)
    {

    }

    void ContinuousCell(int x, int z, int stage, int prevLengthX = 0, int prevLengthZ = 0)
    {
        continuous[x, z, stage] = true;

        int newX = x + 1;
        int newZ = z + 1;

        int continousLengthX = (x < newX && z == newZ) ? prevLengthX + 1 : 1;
        int continousLengthZ = (z < newZ && x == newX) ? prevLengthZ + 1 : 1;

        if (IsInRange(newX, newZ) && !continuous[newX, newZ, stage])
        {
            if (wallObjects[x, stage, z, (int)DIRECTION.NORTH].activeSelf == false)
            {

            }
        }
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    MazeInformation mI;

    GameObject[,] stoneTrapObjects;
    int availableStoneTrap;
    
    void Awake()
    {
        stoneTrapAmount = 0;
    }

    void Start()
    {
        availableStoneTrap = mI.runways.Count;
    }

    public void GenerateStoneTrap()
    {
        for (int i = 0; i < stageLength; i++)
        {
            for (int j = 0; j < stoneTrapAmount; j++)
            {
                int r = Random.Range(0, stageOfTrapAmount(i));
            }
        }
    }

    int stageOfTrapAmount(int stage)
    {
        int amount = mI.runways.Where(n => n.start.y == stage).Count();

        return amount;
    }

    void CreateStoneTrap(int index)
    {

    }
}

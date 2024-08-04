using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureGenerator : MazeComponent
{
    [SerializeField] GameObject treasurePrefab;

    MazeInformation mI;
    MazeGenerator mG;
    GameObject treasureObject;
    GameObject[] stageForTreasure;
    List<GameObject> treasureList = new List<GameObject>();

    void Awake()
    {
        mI = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();
        mG = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        stageForTreasure = new GameObject[stageLength];
    }

    public void GenerateTreasure()
    {
        for (int i = 0; i < stageLength; i++)
        {
            stageForTreasure[i] = new GameObject($"Stage{i + 1}");
            stageForTreasure[i].transform.parent = transform;
        }
        for (int j = 0; j < mI.treasureList.Count; j++)
        {
            CreateTreasure(j);
        }
        for (int k = 0; k < stageLength; k++)
        {
            MoveStageForTreasure(k);
        }
    }

    void CreateTreasure(int id)
    {
        Vector3 pos = mI.treasureList[id];
        treasureObject = Instantiate(treasurePrefab, transform.TransformDirection(pos), Quaternion.Euler(0, 180, 0));
        treasureObject.name = $"Treasure {id}";
        treasureObject.transform.parent = stageForTreasure[(int)pos.y].transform;
        treasureList.Add(treasureObject);
    }

    void MoveStageForTreasure(int stage)
    {
        if (stage == 0)
        {
            return;
        }
        else
        {
            stageForTreasure[stage].transform.position += mG.StairPos(stage) + mG.StairRotToPos(stage);
            stageForTreasure[stage].transform.rotation = mG.StairRotToStageRot(stage);
        }
    }
}

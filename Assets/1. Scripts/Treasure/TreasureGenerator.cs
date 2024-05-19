using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureGenerator : MonoBehaviour
{
    [SerializeField] GameObject treasurePrefab;

    MazeInformation mI;
    GameObject treasureObject;
    List<GameObject> treasureList = new List<GameObject>();

    void Awake()
    {
        mI = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();
    }

    public void GenerateTreasure()
    {
        for (int i = 0; i < mI.treasureList.Count; i++)
        {
            CreateTreasure(i);
        }
    }

    void CreateTreasure(int id)
    {
        Vector3 pos = mI.treasureList[id];
        treasureObject = Instantiate(treasurePrefab, transform.TransformDirection(pos), Quaternion.identity);
        treasureObject.name = $"Treasure {id}";
        treasureObject.transform.parent = transform;
        treasureList.Add(treasureObject);
    }
}

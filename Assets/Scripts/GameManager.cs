using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    public bool isMapInfoCreate { get; private set; }
    public bool isMazeGenerate { get; private set; }

    void Awake()
    {
        i = this;
    }

    void Start()
    {
        MapInfoCreate();
    }

    void MapInfoCreate()
    {
        isMapInfoCreate = true;
        GameObject.Find("MapManager").GetComponent<MapManager>().GenerateInfo();
    }

    void MazeCreate()
    {
        isMazeGenerate = true;
    }
}

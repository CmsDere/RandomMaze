using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    MazeGenerator mazeGen;
    MazeInformation mazeInfo;
    TrapGenerator trapGen;
    PlayerManager playMan;

    void Awake()
    {
        mazeGen = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        mazeInfo = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();
        trapGen = GameObject.Find("TrapGenerator").GetComponent<TrapGenerator>();
        playMan = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

    void Start()
    {
        mazeGen.GenerateMaze();
        mazeInfo.CreateStoneTrapInfo();
        trapGen.GenerateStoneTrap();
        playMan.GeneratePlayer();
    }
}

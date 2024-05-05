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
        // 생성
        mazeGen.GenerateMaze();
        mazeInfo.GenerateStoneTrapInfo();
        trapGen.GenerateTrapBase();
        trapGen.GenerateStoneTrap();

        // 검증

        // 플레이어 스폰
        playMan.GeneratePlayer();
    }
}

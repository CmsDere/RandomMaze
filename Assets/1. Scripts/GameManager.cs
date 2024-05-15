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
        // 정보 생성
        mazeInfo.GenerateStoneTrapInfo();
        mazeInfo.GenerateArrowTrapInfo();
        mazeInfo.GenerateSwampTrapInfo2();
        //mazeInfo.GenerateFlameTrapInfo();
        //mazeInfo.RemoveDuplicatePositionWithOtherTrap();
        // 함정 생성
        trapGen.GenerateTrapBase();
        trapGen.GenerateStoneTrap();
        trapGen.GenerateArrowTrap();
        trapGen.GenerateSwampTrap();
        //trapGen.GenerateFlameTrap();
        // 플레이어 스폰
        playMan.GeneratePlayer();
    }
}

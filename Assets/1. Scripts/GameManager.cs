using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    MazeGenerator mazeGen;
    MazeInformation mazeInfo;
    TrapGenerator trapGen;
    TreasureGenerator treaGen;
    PlayerManager playMan;

    public bool isPlayerSpawn { get; set; }

    void Awake()
    {
        i = this;

        mazeGen = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        mazeInfo = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();
        trapGen = GameObject.Find("TrapGenerator").GetComponent<TrapGenerator>();
        treaGen = GameObject.Find("TreasureGenerator").GetComponent<TreasureGenerator>();
        playMan = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

    void Start()
    {
        // ����
        mazeGen.GenerateMaze();
        // ���� ����
        mazeInfo.GenerateStoneTrapInfo();
        mazeInfo.GenerateArrowTrapInfo();
        mazeInfo.GenerateSwampTrapInfo();
        mazeInfo.GenerateFlameTrapInfo();
        // ���� ����
        trapGen.GenerateTrapBase();
        trapGen.GenerateStoneTrap();
        trapGen.GenerateArrowTrap();
        trapGen.GenerateSwampTrap();
        trapGen.GenerateFlameTrap();
        // ���� ���� ����
        mazeInfo.GenerateTreasureInfo();
        // ���� ����
        treaGen.GenerateTreasure();
        // �÷��̾� ����
        playMan.GeneratePlayer();
    }
}

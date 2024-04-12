using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    [Header("함정 생성 정보")]
    [SerializeField] int trapAmount = 20;

    [Header("함정 생성 프리팹")]
    [SerializeField] GameObject tempTrapPrefab;    
}

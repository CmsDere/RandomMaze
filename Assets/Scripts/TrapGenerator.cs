using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    [Header("���� ���� ����")]
    [SerializeField] int trapAmount = 20;

    [Header("���� ���� ������")]
    [SerializeField] GameObject tempTrapPrefab;    
}

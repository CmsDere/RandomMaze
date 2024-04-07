using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [Header("화살 함정 관련 변수")]
    [SerializeField] int arrowCount = 10;

    [Header("화살 함정 관련 프리팹")]
    [SerializeField] GameObject arrowPrefab;

    GameObject[,,,] wallObjects;
    GameObject[] arrowObjects;

    void Start()
    {
        wallObjects = GetComponent<MazeGenerator>().wallObjects;
        arrowObjects = new GameObject[arrowCount];
    }

    void CreateArrow()
    {
        for (int i = 0; i < arrowCount; i++)
        {
            arrowObjects[i] = Instantiate
                (
                    arrowPrefab,
                    wallObjects[1, 1, 1, 1].transform.position,
                    Quaternion.identity
                );
        }
    }
}

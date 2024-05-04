using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTrigger : MonoBehaviour
{
    public bool isCellTrigger { get; private set; }

    TrapGenerator trapGen;

    void Start()
    {
        isCellTrigger = false;
        trapGen = GameObject.Find("TrapGenerator").GetComponent<TrapGenerator>();
    }
}

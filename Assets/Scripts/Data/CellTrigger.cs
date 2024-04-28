using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTrigger : MonoBehaviour
{
    public bool isCellTrigger { get; private set; }

    void Start()
    {
        isCellTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCellTrigger = true;
        }
    }
}

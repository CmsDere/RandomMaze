using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTrap : MonoBehaviour
{
    [SerializeField] GameObject stone;

    public bool isStoneTrapActive { get; private set; }

    void Start()
    {
        isStoneTrapActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isStoneTrapActive = true;
        }     
    }
 
}

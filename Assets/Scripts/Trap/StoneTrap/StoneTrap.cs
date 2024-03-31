using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTrap : MonoBehaviour
{
    [SerializeField] GameObject stone;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        stone.GetComponent<Rigidbody>().isKinematic = false;
    }
 
}

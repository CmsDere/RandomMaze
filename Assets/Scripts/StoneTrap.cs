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
        if (other.gameObject == GameObject.Find("ActiveTrap"))
        {
            Debug.Log("Active");
            stone.GetComponent<Rigidbody>().isKinematic = false;
            stone.GetComponent<Rigidbody>().detectCollisions = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == GameObject.Find("ActiveTrap"))
        {
            Debug.Log("Active2");
            stone.GetComponent<Rigidbody>().isKinematic = false;
            stone.GetComponent<Rigidbody>().detectCollisions = false;
        }
    }
}

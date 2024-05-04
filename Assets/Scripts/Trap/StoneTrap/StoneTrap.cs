using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTrap : MonoBehaviour
{
    public bool isStoneTrapActive { get; private set; }
    public Vector3 start { get; set; }
    public Vector3 end { get; set; }
    public string direction { get; set; }

    [SerializeField] GameObject stonePrefab;

    GameObject stone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CreateStone(other.transform.position);
        }     
    }
 
    void CreateStone(Vector3 pos)
    {
        Vector3 stoneStartPos = new Vector3(0, 0.5f, 0);
        if (direction == "Horizontal")
        {
            if (pos.x - (start.x - 0.5f) < (end.x + 0.5f) - pos.x)
            {
                stone = Instantiate(stonePrefab, end + stoneStartPos, Quaternion.identity, transform);
            }
            else if (pos.x - (start.x - 0.5f) > (end.x + 0.5f) - pos.x)
            {
                stone = Instantiate(stonePrefab, start + stoneStartPos, Quaternion.identity, transform);
            }
        }
        else if (direction == "Vertical")
        {
            if (pos.z - (start.z - 0.5f) < (end.z + 0.5f) - pos.z)
            {
                stone = Instantiate(stonePrefab, end + stoneStartPos, Quaternion.identity, transform);
            }
            else if (pos.z - (start.z - 0.5f) > (end.z + 0.5f) - pos.z)
            {
                stone = Instantiate(stonePrefab, start + stoneStartPos, Quaternion.identity, transform);
            }
        }
    }
}

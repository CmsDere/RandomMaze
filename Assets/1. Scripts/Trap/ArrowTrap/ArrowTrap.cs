using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [Header("화살 함정 관련 변수")]
    [SerializeField] float moveSpeed = 1f;

    [Header("화살 함정 관련 프리팹")]
    [SerializeField] GameObject arrowPrefab;

    GameObject arrowObject;
    bool isPlayerEnter = false;

    public Vector3 arrowTrapPos { get; set; }
    public string arrowTrapDirection { get; set; }

    void Update()
    {
        if (isPlayerEnter)
        {
            ShootArrow();
        }
    }

    void CreateArrow()
    {
        if (arrowTrapDirection == "South")
        {
            arrowObject = Instantiate(arrowPrefab, new Vector3(0, 0, 0.2f), Quaternion.Euler(0, 90, 0), transform);
        }
        else if (arrowTrapDirection == "East")
        {
            arrowObject = Instantiate(arrowPrefab, new Vector3(0.2f, 0, 0), Quaternion.identity, transform);
        }
    }

    void ShootArrow()
    {
        if (arrowTrapDirection == "South")
        {
            arrowObject.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        else if (arrowTrapDirection == "East")
        {
            arrowObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerEnter = true;
            if (isPlayerEnter) CreateArrow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerEnter = false;
        }
        else if (other.gameObject.CompareTag("Arrows"))
        {
            CreateArrow();
        }
    }
}

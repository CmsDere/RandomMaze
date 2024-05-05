using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    // 공이 굴러가는 방향
    public string direction { get; set; }

    void Update()
    {
        if (this.gameObject != null)
        {
            MoveStone();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        //Debug.Log(collision.gameObject.name);
    }

    void MoveStone()
    {
        if (direction == "North") this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        else if (direction == "South") this.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        else if (direction == "West") this.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        else if (direction == "East") this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }
}

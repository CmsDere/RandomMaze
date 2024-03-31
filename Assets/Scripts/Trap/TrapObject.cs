using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // 돌 함정이 플레이어 혹은 벽에 부딪혔을 때 돌 오브젝트 제거
        if (collision.gameObject == GameObject.FindWithTag("Player") || collision.gameObject == GameObject.FindWithTag("Wall"))
        {
            Debug.Log(collision.gameObject.name);
            Destroy(this.gameObject);
        }

    }
}

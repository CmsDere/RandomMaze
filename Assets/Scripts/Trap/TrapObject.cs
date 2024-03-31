using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // �� ������ �÷��̾� Ȥ�� ���� �ε����� �� �� ������Ʈ ����
        if (collision.gameObject == GameObject.FindWithTag("Player") || collision.gameObject == GameObject.FindWithTag("Wall"))
        {
            Debug.Log(collision.gameObject.name);
            Destroy(this.gameObject);
        }

    }
}

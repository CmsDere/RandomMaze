using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 1f;

    float xRotate, yRotate;

    void Update()
    {
        
        xRotate += -Input.GetAxis("Mouse Y") * rotateSpeed;
        xRotate = Mathf.Clamp(xRotate, -60, 90);

        transform.eulerAngles = new Vector3(
            xRotate, 0, 0
            );
    }
}

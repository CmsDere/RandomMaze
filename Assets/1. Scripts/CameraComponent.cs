using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraComponent : MonoBehaviour
{
    [SerializeField] float sensitivity = 500f;
    Material outlineMat;

    float rotationX;
    float rotationY;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        outlineMat = new Material(Shader.Find("Outline/PostprocessOutline"));
    }

    void Update()
    {
        Move();
    }

    void FixedUpdate()
    {
        Interact();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");
        rotationY += moveX * sensitivity * Time.deltaTime;
        rotationX += moveY * sensitivity * Time.deltaTime;

        if (rotationX > 35)
            rotationX = 35;
        if (rotationX < -30)
            rotationX = -30;

        transform.eulerAngles = new Vector3(-rotationX, rotationY, 0);
    }

    void Interact()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Treasure")
            {
                Debug.Log("Treasure");
                hit.transform.GetComponent<Renderer>().material = outlineMat;
            }
        }
        
    }
}

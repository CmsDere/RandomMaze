using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;
public class CameraComponent : MonoBehaviour
{
    [SerializeField] float sensitivity = 500f;
    
    float rotationX;
    float rotationY;
    private Camera cam;
    Vector3 center;

    void Awake()
    {
        cam = GetComponent<Camera>();
        center = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2);
    }

    void Update()
    {
        Move();
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
        GameObject target = null;
            
        Ray ray = cam.ScreenPointToRay(center);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * hit.distance, Color.red);
            if (hit.transform.gameObject == GameObject.FindWithTag("Treasure"))
            {
                target = hit.transform.gameObject;
                target.GetComponent<TreasureBox>().SelectBox();
            }
            else
            {
                if (target != null)
                {
                    if (target == GameObject.FindWithTag("Treasure"))
                    {
                        target.GetComponent<TreasureBox>().DeselectBox();
                    }
                }
                else
                {

                }
            }
        }
        else
        {
            GameObject.FindWithTag("Treasure").GetComponent<TreasureBox>().DeselectBox();
        }
    }
}

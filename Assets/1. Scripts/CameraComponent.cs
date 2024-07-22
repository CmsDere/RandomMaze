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

    GameObject target;

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
            
        Ray ray = cam.ScreenPointToRay(center);

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * hit.distance, Color.red);
            if (hit.collider.gameObject.CompareTag("Treasure"))
            {
                target = hit.collider.gameObject;
                target.GetComponent<TreasureBox>().isSelect = true;
                target.GetComponent<TreasureBox>().SelectBox();
            }
            else
            {
                if (UIManager.instance.IsOpenedUI(UIType.InteractUI))
                {
                    UIManager.instance.CloseUI(UIType.InteractUI);
                }
                return;
            }
        }
        else
        {
            if (target != null)
            {
                if (target.CompareTag("Treasure"))
                {
                    target.GetComponent<TreasureBox>().isSelect = false;
                    target.GetComponent<TreasureBox>().DeselectBox();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}

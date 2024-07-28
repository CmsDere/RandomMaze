using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;
    bool isExecute = false;

    Material outlineMat;
    [SerializeField] Material originalMat;

    Animator animator;

    void Start()
    {
        outlineMat = new Material(Shader.Find("Custom/Outline"));
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isSelect)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 아이템 획득
                GetItem();
            }
        }
    }

    public void SelectBox()
    {
        Transform[] allChild = GetComponentsInChildren<Transform>();

        if (isSelect && !isExecute)
        {
            foreach(Transform child in allChild)
            {
                child.gameObject.GetComponent<Renderer>().material = outlineMat;
            }
            UIManager.instance.OpenUI(UIType.InteractUI);
            isExecute = true;
        }
    }

    public void DeselectBox()
    {
        Transform[] allchild = GetComponentsInChildren<Transform>();

        if(!isSelect && isExecute)
        {
            foreach(Transform child in allchild)
            {
                child.gameObject.GetComponent<Renderer>().material = originalMat;
            }
            if (UIManager.instance.IsOpenedUI(UIType.InteractUI))
            {
                UIManager.instance.CloseUI(UIType.InteractUI);
            }
            isExecute = false;
        }
    }

    void GetItem()
    {
        // 상자 열리는 애니메이션
        animator.SetBool("IsOpen", true);
        // 아이템의 실제 오브젝트 생성

    }

}

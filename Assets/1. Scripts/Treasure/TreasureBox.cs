using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;
using DefineTable;
using DefinedItem;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;
    bool isExecute = false;
    bool isOpen = false;

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
        if (!isOpen)
        {
            isOpen = true;
            StartCoroutine(GetItemRoutine());
        }
        else
        {
            Debug.Log("이미 열린 상자입니다.");
        }
    }

    IEnumerator GetItemRoutine()
    {
        animator.SetBool("IsOpen", true);
        ActiveItemObject();
        UIManager.instance.OpenUI(UIType.GetItemUI);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        if (UIManager.instance.IsOpenedUI(UIType.GetItemUI))
        {
            UIManager.instance.CloseUI(UIType.GetItemUI);
        }
        yield break;
    }

    void ActiveItemObject()
    {
        int itemCount = transform.childCount - 4;
        int drop = Random.Range(0, itemCount);

        transform.GetChild(drop + 4).gameObject.SetActive(true);
    }
}

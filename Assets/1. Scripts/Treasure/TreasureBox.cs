using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;
    bool isExecute = false;

    Renderer renderers;

    Material outlineMat;
    [SerializeField] Material originalMat;

    void Start()
    {
        renderers = GetComponent<Renderer>();
        outlineMat = new Material(Shader.Find("Custom/Outline"));
    }

    void Update()
    {
        if (isSelect)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // ������ ȹ��
            }
        }
    }

    public void SelectBox()
    {
        if (isSelect && !isExecute)
        {
            renderers.material = outlineMat;
            UIManager.instance.OpenUI(UIType.InteractUI);
            isExecute = true;
        }
    }

    public void DeselectBox()
    {
        if(!isSelect && isExecute)
        {
            renderers.material = originalMat;
            if (UIManager.instance.IsOpenedUI(UIType.InteractUI))
            {
                UIManager.instance.CloseUI(UIType.InteractUI);
            }
            isExecute = false;
        }
    }

    void GetItem()
    {
        // ���� ������ �ִϸ��̼�
        // �������� ���� ������Ʈ ����
    }

}

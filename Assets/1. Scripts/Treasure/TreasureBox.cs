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
                // æ∆¿Ã≈€ »πµÊ
            }
        }
    }

    public void SelectBox()
    {
        if (isSelect && !isExecute)
        {
            renderers.material = outlineMat;
            UIManager._instance.OpenUI(UIType.InteractUI);
            isExecute = true;
        }
    }

    public void DeselectBox()
    {
        if(!isSelect && isExecute)
        {
            renderers.material = originalMat;
            if (UIManager._instance.IsOpenedUI(UIType.InteractUI))
            {
                UIManager._instance.CloseUI(UIType.InteractUI);
            }
            isExecute = false;
        }
    }

    void GetItem()
    {

    }

}

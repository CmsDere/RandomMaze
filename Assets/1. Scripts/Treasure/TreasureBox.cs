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
                // 아이템 획득
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
        // 상자 열리는 애니메이션
        // 아이템의 실제 오브젝트 생성
    }

}

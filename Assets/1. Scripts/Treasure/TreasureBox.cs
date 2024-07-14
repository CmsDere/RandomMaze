using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;

    Renderer renderers;

    Material outlineMat;
    [SerializeField] Material originalMat;

    void Start()
    {
        renderers = GetComponent<Renderer>();
        outlineMat = new Material(Shader.Find("Custom/Outline"));
    }

    public void SelectBox()
    {
        renderers.material = outlineMat;
        if (!UIManager._instance.IsOpenedUI(UIType.InteractUI))
        {
            UIManager._instance.OpenUI(UIType.InteractUI);
        }
    }

    public void DeselectBox()
    {
        renderers.material = originalMat;
        if (UIManager._instance.IsOpenedUI(UIType.InteractUI))
        {
            UIManager._instance.CloseUI(UIType.InteractUI);
        }
    }

}

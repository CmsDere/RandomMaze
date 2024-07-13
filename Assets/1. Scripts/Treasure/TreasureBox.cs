using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;

    Renderer renderers;
    Material outlineMat;
    Material originalMat;

    void Start()
    {
        outlineMat = new Material(Shader.Find("Custom/Outline"));
        renderers = GetComponent<Renderer>();
        originalMat = renderers.sharedMaterial;
    }

    void Update()
    {
        if (isSelect)
        {
            renderers.material = outlineMat;
            //UIManager._instance.OpenUI(UIType.INTERACT_UI);
        }
        else
        {
            renderers.material = originalMat;
            //UIManager._instance.CloseUI(UIType.INTERACT_UI);
            
        }
    }
}

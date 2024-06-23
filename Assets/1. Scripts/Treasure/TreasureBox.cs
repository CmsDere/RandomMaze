using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;
    bool isChange = true;

    Renderer renderers;
    [SerializeField] List<Material> materials = new List<Material>();
    Material outlineMat;

    void Start()
    {
        outlineMat = new Material(Shader.Find("Outline/PostprocessOutline"));
        renderers = GetComponent<Renderer>();
        materials.Add(renderers.sharedMaterial);
        materials.Add(outlineMat);
    }

    void Update()
    {
        if (isSelect)
        {
            renderers.material = materials[1];
            UIManager.i.OpenUI(UIType.INTERACT_UI);
        }
        else
        {
            renderers.material = materials[0];
            UIManager.i.CloseUI(UIType.INTERACT_UI);
            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public bool isSelect = false;
    bool isChange = true;

    Renderer renderers;
    List<Material> materials = new List<Material>();
    Material outlineMat;

    void Start()
    {
        outlineMat = new Material(Shader.Find("Outline/PostprocessOutline"));
        renderers = GetComponent<Renderer>();
    }

    void Update()
    {
        if (isSelect)
        {
            materials.Clear();
            materials.AddRange(renderers.sharedMaterials);
            materials.Add(outlineMat);

            renderers.materials = materials.ToArray();
            isChange = false;
            Debug.Log("T");
        }
        else
        {
            materials.Clear();
            materials.AddRange(renderers.sharedMaterials);
            materials.Remove(outlineMat);

            renderers.materials = materials.ToArray();
            isChange = true;
            Debug.Log("F");
        }
    }
}

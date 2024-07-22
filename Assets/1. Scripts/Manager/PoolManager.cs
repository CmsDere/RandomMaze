using DefineUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : TSingleton<PoolManager>
{
    string uiPath = "UIPrefab/";
    protected override void Init()
    {
        base.Init();
    }

    public void Create(UIType uiType, Transform parent)
    {
        GameObject uiPrefab = Resources.Load(uiPath + uiType.ToString()) as GameObject;

        if (uiPrefab == null)
        {
            Debug.Log($"{uiType}�� �������� ����!!");
            return;
        }

        Instantiate(uiPrefab, parent);
    }
}

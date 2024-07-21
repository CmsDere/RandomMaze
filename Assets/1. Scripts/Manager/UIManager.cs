using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;
using UnityEditor;

public class UIManager : TSingleton<UIManager>
{
    Dictionary<UIType, UIBase> uiDatas;
    
    [SerializeField] GameObject uiPrefab;

    protected override void Init()
    {
        base.Init();
        uiDatas = new Dictionary<UIType, UIBase>();

        Debug.Log("Init");
    }

    public void OpenUI(UIType ui)
    {
        if(uiDatas.ContainsKey(ui))
        {
            uiDatas[ui].Open();
        }
        else
        {
            UIBase uiBase = CreateUI(ui);
            if (uiBase != null)
                uiDatas.Add(ui, uiBase);
            else
                Debug.Log($"{ui}�� �������� ����");
        }
    }

    public void CloseUI(UIType ui)
    {
        if (uiDatas.ContainsKey(ui))
        {
            uiDatas[ui].Close();
        }
        else
            Debug.Log("�������� ���� UI��.");
    }

    public bool IsOpenedUI(UIType ui)
    {
        if (uiDatas.ContainsKey(ui))
            return uiDatas[ui].gameObject.activeSelf;
        else
            return false;
    }

    public UIBase CreateUI(UIType type)
    {
        UIBase uiBase = null;
        string path = "UIPrefabs/";
        //GameObject uiPrefab = Resources.Load(path + type.ToString()) as GameObject;

        if (uiPrefab == null)
        {
            Debug.Log($"{type}�� �������� ����");
            return null;
        }

        GameObject go = Instantiate(uiPrefab, transform);

        switch(type)
        {
            case UIType.BuffUI:
                break;
            case UIType.EquipUI:
                break;
            case UIType.FoodUI:
                break;
            case UIType.HPUI:
                break;
            case UIType.InventoryUI:
                break;
            case UIType.SwapUI:
                {
                    SwapUI ui = go.GetComponent<SwapUI>();
                    uiBase = ui;
                }
                break;
            case UIType.TimeUI:
                break;
            case UIType.InteractUI:
                {
                    InteractUI ui = go.GetComponent<InteractUI>();
                    uiBase = ui;
                }
                break;
        }

        return uiBase;
    }
}

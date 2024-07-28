using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;
using UnityEditor;

public class UIManager : TSingleton<UIManager>
{
    Dictionary<UIType, UIBase> uiDatas;

    protected override void Init()
    {
        base.Init();
        Debug.Log("UIManager");
        uiDatas = new Dictionary<UIType, UIBase>();

        GenerateUI();
    }

    void GenerateUI()
    {
        CreateOpenUI(UIType.HPUI);
        CreateCloseUI(UIType.InteractUI);
        CreateCloseUI(UIType.GetItemUI);
    }

    public void CreateCloseUI(UIType type)
    {
        CreateUI(type);
        uiDatas[type].Close();
    }

    public void CreateOpenUI(UIType type)
    {
        CreateUI(type);
    }

    void CreateUI(UIType type)
    {
        UIBase ui = CreateBase(type);
        if (ui != null)
        {
            uiDatas.Add(type, ui);
        }
    }

    public void OpenUI(UIType ui)
    {
        if(uiDatas.ContainsKey(ui))
        {
            uiDatas[ui].Open();
        }
        else
        {
            Debug.Log($"{ui}가 생성되지 않음");
        }
    }

    public void CloseUI(UIType ui)
    {
        if (uiDatas.ContainsKey(ui))
        {
            uiDatas[ui].Close();
        }
        else
            Debug.Log("생성되지 않은 UI임.");
    }

    public bool IsOpenedUI(UIType ui)
    {
        if (uiDatas.ContainsKey(ui))
            return uiDatas[ui].gameObject.activeSelf;
        else
            return false;
    }

    public UIBase CreateBase(UIType type)
    {
        UIBase uiBase = null;
        GameObject uiPrefab = Resources.Load("UIPrefabs/" + type.ToString()) as GameObject;

        if (uiPrefab == null)
        {
            Debug.Log($"{type}이 존재하지 않음");
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
                {
                    HPUI ui = go.GetComponent<HPUI>();
                    uiBase = ui;
                }
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
            case UIType.GetItemUI:
                {
                    GetItemUI ui = go.GetComponent<GetItemUI>();
                    uiBase = ui;
                }
                break;
        }

        return uiBase;
    }
}

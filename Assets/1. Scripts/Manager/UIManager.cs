using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineUI;

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    Dictionary<UIType, UIBase> uiDatas = new Dictionary<UIType, UIBase>();

    void Awake()
    {
        i = this;
    }

    public void OpenUI(UIType ui)
    {
        if(uiDatas.ContainsKey(ui))
        {
            uiDatas[ui].UIOpen();
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

    public UIBase CreateUI(UIType type)
    {
        UIBase uiBase = null;
        string path = "2.Prefabs/UI/";
        GameObject uiPrefab = Resources.Load(path + type.ToString()) as GameObject;

        if (uiPrefab == null)
        {
            Debug.Log($"{type.ToString()}�� �������� ����");
            return null;
        }

        GameObject go = Instantiate(uiPrefab, transform);

        switch(type)
        {
            case UIType.BUFF_UI:
                break;
            case UIType.EQUIP_UI:
                break;
            case UIType.FOOD_UI:
                break;
            case UIType.HP_UI:
                break;
            case UIType.INVENTORY_UI:
                break;
            case UIType.SWAP_UI:
                {
                    SwapUI ui = go.GetComponent<SwapUI>();
                    uiBase = ui;
                }
                break;
            case UIType.TIME_UI:
                break;
        }

        return uiBase;
    }
}

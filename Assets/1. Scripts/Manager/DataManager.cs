using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineTable;

public class DataManager : TSingleton<DataManager>
{
    Dictionary<TableName, TableBase> tableList;

    protected override void Init()
    {
        base.Init();
        tableList = new Dictionary<TableName, TableBase>();
    }

    bool Load<T>(TableName name) where T : TableBase, new()
    {
        if (tableList.ContainsKey(name))
            return true;

        string path = "Data/";
        TextAsset tAsset = Resources.Load(path + name) as TextAsset;

        if (tAsset != null)
        {
            T t = new T();
            t.Load(tAsset.text);
            tableList.Add(name, t);
        }
        else
        {
            return false;
        }

        return true;
    }

    public void LoadAll()
    {
        if (!Load<ItemTable>(TableName.ItemTable))
        {
            Debug.Log($"{TableName.ItemTable.ToString()} 로드에 실패하였습니다.");
        }
    }

    public TableBase Get(TableName name)
    {
        if (tableList.ContainsKey(name))
        {
            return tableList[name];
        }

        return null;
    }
}

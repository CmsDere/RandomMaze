using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnums;

public class DataManager2 : TSingleton<DataManager2>
{
    Dictionary<TableName, TableBase2> _tableList;

    protected override void Init()
    {
        base.Init();
        _tableList = new Dictionary<TableName, TableBase2>();
    }

    bool Load<T>(TableName name) where T : TableBase2, new()
    {
        if (_tableList.ContainsKey(name))
            return true;

        string path = "TableDatas/";
        TextAsset tAsset = Resources.Load(path + name) as TextAsset;

        if (tAsset != null)
        {
            T t = new T();
            t.Load(tAsset.text);
            _tableList.Add(name, t);
        }
        else
        {
            return false;
        }

        return true;
    }

    public void LoadAll()
    {
        if (!Load<ItemTable2>(TableName.ItemTable))
        {
            Debug.LogFormat("[{0}] 로드에 실패하였습니다.", TableName.ItemTable.ToString());
        }
        else if (!Load<MonsterTable2>(TableName.MonsterTable))
        {
            Debug.LogFormat("[{0}] 로드에 실패하였습니다.", TableName.ItemTable.ToString());
        }
    }

    public TableBase2 Get(TableName name)
    {
        if (_tableList.ContainsKey(name))
        {
            return _tableList[name];
        }
        return null;
    }
}

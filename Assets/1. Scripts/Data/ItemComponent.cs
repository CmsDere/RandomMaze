using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    Dictionary<string, Dictionary<string, string>> itemTable;

    void Awake()
    {
        DataManager._instance.LoadAll();
    }

    //DataManager2._instance.Get(TableName.ItemTable).ToString(3, ItemTable2.ColumnName.Explain.ToString()));
    //DataManager2._instance.Get(TableName.MonsterTable).ToString(3, MonsterTable2.MonsterTableColumnName.Defence.ToString()));
}



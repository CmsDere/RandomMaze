using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefineEnums;

public class Test : MonoBehaviour
{
    Dictionary<string, Dictionary<string, string>> _itemTable;

    void Awake()
    {
        DataManager2._instance.LoadAll();
    }

    void Start()
    {
        //TextAsset tAsset = Resources.Load("TableDatas/" + "ItemTable") as TextAsset;

        //ReadTable(tAsset.text);

        //Debug.Log(GetTableValue(4,"Name"));

        Debug.Log(DataManager2._instance.Get(TableName.ItemTable).ToString(3, ItemTable2.ColumnName.Explain.ToString()));
        Debug.Log(DataManager2._instance.Get(TableName.MonsterTable).ToString(3, MonsterTable2.MonsterTableColumnName.Defence.ToString()));
    }

    void ReadTable(string jsonData)
    {
        _itemTable = new Dictionary<string, Dictionary<string, string>>();

        string[] datas = jsonData.Split('[', ']', StringSplitOptions.RemoveEmptyEntries);
        string[] field = datas[1].Split('{', '}', StringSplitOptions.RemoveEmptyEntries);

        for (int n = 0; n < field.Length; n++)
        {
            Dictionary<string, string> record = new Dictionary<string, string>();
            string[] data = field[n].Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int m = 0; m < data.Length; m++)
            {
                string buff = data[m].Replace("{", "");
                string[] pairData = buff.Split(':');
                record.Add(ExtraReplace(pairData[0]), ExtraReplace(pairData[0]));
            }
            _itemTable.Add(record["Index"], record);
        }
    }

    string ExtraReplace(string buff)
    {
        string val = buff.Replace("\"", "");
        string re = val;
        if (val.Contains("\\u"))
        {
            val.Remove(0, 1);
            re = string.Empty;
            for (int n = 0; n < val.Length; n++)
            {
                if (val[n] == '\\' && val[n + 1] == 'u')
                {
                    string hax = val.Substring(n + 2, 4);
                    int data = Convert.ToInt32(hax, 16);
                    n += 5;
                    re += char.ConvertFromUtf32(data);
                }
                else
                {
                    re += val[n];
                }
            }
        }
        return re;
    }

    string GetTableValue(int index, string column)
    {
        string tableValue = "";
        

        return tableValue;
    }
}

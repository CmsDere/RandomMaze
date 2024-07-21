using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParceJSON;

public class ItemTable2 : TableBase2
{
    public enum ColumnName
    {
        Index,
        Name,
        UseType,
        AddATT,
        AddDEF,
        AddHP,
        Price,
        Explain,

        Max
    }

    string _mainKey = "Index";

    public override void Load(string jsonData)
    {
        JSONNode node = JSONNode.Parse(jsonData);

        for (int n = 0; n < (int)ColumnName.Max; n++)
        {
            ColumnName subKey = (ColumnName)n;
            if (string.Compare(_mainKey, subKey.ToString()) != 0)
            {
                for (int m = 0; m < node[0].AsArray.Count; m++)
                {
                    Add(node[0][m][_mainKey], subKey.ToString(), node[0][m][subKey.ToString()].Value);
                }
            }
        }
    }
}

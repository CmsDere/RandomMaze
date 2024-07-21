using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParceJSON;

public class ItemTable : TableBase
{
    string _mainKey = "Index";

    public enum ItemTableColumnName
    {
        Index,
        Name,
        ItemType,
        Effect,
        EffectValue,
        Description,

        Max
    }

    public override void Load(string jsonData)
    {
        JSONNode node = JSONNode.Parse(jsonData);

        for (int n = 0; n < (int)ItemTableColumnName.Max; n++)
        {
            ItemTableColumnName subKey = (ItemTableColumnName)n;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParceJSON;

public class MonsterTable2 : TableBase2
{
    public enum MonsterTableColumnName
    {
        Index,
        Name,
        Rank,
        Level,
        Tribe,
        AttackType,
        HP,
        Attack,
        Defence,
        DropItem1,
        DropItem2,
        DropItem3,
        DropItem4,
        DropRate,

        Max
    }

    string _mainKey = "Index";

    public override void Load(string jsonData)
    {
        JSONNode node = JSONNode.Parse(jsonData);

        for (int n = 0; n < (int)MonsterTableColumnName.Max; n++)
        {
            MonsterTableColumnName subKey = (MonsterTableColumnName)n;
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

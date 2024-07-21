using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TableBase : MonoBehaviour
{
    Dictionary<string, Dictionary<string, string>> tableDatas;

    public int maxCount { get { return tableDatas.Count; } }
    
    public TableBase()
    {
        tableDatas = new Dictionary<string, Dictionary<string, string>>();
    }

    public abstract void Load(string jsonData);
    protected void Add(string key, string subKey, string val)
    {
        if (!tableDatas.ContainsKey(key))
        {
            tableDatas.Add(key, new Dictionary<string, string>());
        }

        if (!tableDatas[key].ContainsKey(subKey))
        {
            tableDatas[key].Add(subKey, val);
        }
    }

    public string ToString(int key, string subKey)
    {
        return ToString(key.ToString(), subKey);
    }

    public string ToString(string key, string subKey)
    {
        string findValue = string.Empty;
        if (tableDatas.ContainsKey(key))
        {
            tableDatas[key].TryGetValue(subKey, out findValue);
        }

        return findValue;
    }
}

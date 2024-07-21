using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TableBase2 : MonoBehaviour
{
    Dictionary<string, Dictionary<string, string>> _tableDatas;

    public int _maxCount { get { return _tableDatas.Count; } }

    public TableBase2()
    {
        _tableDatas = new Dictionary<string, Dictionary<string, string>>();
    }

    public abstract void Load(string jsonData);
    protected void Add(string key, string subKey, string val)
    {
        if (!_tableDatas.ContainsKey(key))
        {
            _tableDatas.Add(key, new Dictionary<string, string>());
        }

        if (!_tableDatas[key].ContainsKey(subKey))
        {
            _tableDatas[key].Add(subKey, val);
        }
    }

    public string ToString(int key, string subKey)
    {
        return ToString(key.ToString(), subKey);
    }

    public string ToString(string key, string subKey)
    {
        string findValue = string.Empty;
        if (_tableDatas.ContainsKey(key))
            _tableDatas[key].TryGetValue(subKey, out findValue);

        return findValue;
    }
}

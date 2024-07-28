using DefineUI;
using DefinedItem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : TSingleton<PoolManager>
{
    Dictionary<ItemName, ItemBase> itemDatas;

    protected override void Init()
    {
        base.Init();
        Debug.Log("PoolManager");
        itemDatas = new Dictionary<ItemName, ItemBase>();

        CreateItem(ItemName.VineApple);
    }

    public void CreateItem(ItemName itemName)
    {
        ItemBase item = CreateBase(itemName);
        if (item != null)
        {
            itemDatas.Add(itemName, item);
        }
    }

    public ItemBase CreateBase(ItemName itemName)
    {
        ItemBase itemBase = null;
        GameObject itemPrefab = Resources.Load("ItemPrefabs/" + itemName.ToString()) as GameObject;

        if (itemPrefab == null)
        {
            Debug.Log($"������ ������ {itemName}�� �������� ����");
            return null;
        }

        GameObject go = Instantiate(itemPrefab, transform);
        go.name = itemName.ToString();

        switch(itemName)
        {
            case ItemName.MazeSpecter:
                break;
            case ItemName.HealSpecter:
                break;
            case ItemName.VineApple:
                {
                    VineApple item = go.GetComponent<VineApple>();
                    itemBase = item;
                }
                break;
            case ItemName.HealBuff:
                break;
            case ItemName.SpeedAura:
                break;
        }

        return itemBase;
    }

    public void SpawnItem(ItemName itemName)
    {
        if (itemDatas.ContainsKey(itemName))
            itemDatas[itemName].Spawn();
        else
            Debug.Log($"{itemName}�� �������� ����");
    }

    public void DespawnItem(ItemName itemName)
    {
        if (itemDatas.ContainsKey(itemName))
            itemDatas[itemName].Despawn();
        else
            Debug.Log("�������� ���� ��������");
    }

    public bool IsSpawnedItem(ItemName itemName)
    {
        if (itemDatas.ContainsKey(itemName))
            return itemDatas[itemName].gameObject.activeSelf;
        else
            return false;
    }
}

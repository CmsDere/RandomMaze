using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData", order = int.MaxValue)]
public class ItemComponent : ScriptableObject
{
    // ���� �ڵ�, ���� �̸�, ���� ����, ȿ��, ȿ����
    [SerializeField] int itemIndex;
    [SerializeField] string itemName;
    [SerializeField] ITEM_TYPE itmeType;
    [SerializeField] BUFF_TYPE buffType;
    [SerializeField] int buffAmount;
}



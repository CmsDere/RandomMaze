using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/ItemData", order = int.MaxValue)]
public class ItemComponent : ScriptableObject
{
    // 보물 코드, 보물 이름, 보물 형태, 효과, 효과값
    [SerializeField] int itemIndex;
    [SerializeField] string itemName;
    [SerializeField] ITEM_TYPE itmeType;
    [SerializeField] BUFF_TYPE buffType;
    [SerializeField] int buffAmount;
}



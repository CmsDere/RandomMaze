using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    // ���� �ڵ�, ���� �̸�, ���� ����, ȿ��, ȿ����
    [SerializeField] int itemIndex;
    [SerializeField] string itemName;
    [SerializeField] Sprite itemSprite;
    [SerializeField] ITEM_TYPE itemType;
    [SerializeField] BUFF_TYPE buffType;
    [SerializeField] int buffAmount;

    protected void UseItem(ITEM_TYPE itemType)
    {
        switch(itemType)
        {
            case ITEM_TYPE.FOOD:
                break;
            case ITEM_TYPE.BUFF_TIMER:
                break;
            case ITEM_TYPE.BUFF_CONST:
                break;
        }
    }
}



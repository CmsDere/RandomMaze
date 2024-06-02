using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : MonoBehaviour
{
    // 보물 코드, 보물 이름, 보물 형태, 효과, 효과값
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



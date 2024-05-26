using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapUI : UIBase
{
    public override void UIOpen()
    {
        base.UIOpen();
        Cursor.visible = true;
    }

    public override void UIClose()
    {
        base.UIClose();
        Cursor.visible = false;
    }
}

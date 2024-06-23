using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapUI : UIBase
{
    public override void Open()
    {
        base.Open();
        Cursor.visible = true;
    }

    public override void Close()
    {
        base.Close();
        Cursor.visible = false;
    }
}

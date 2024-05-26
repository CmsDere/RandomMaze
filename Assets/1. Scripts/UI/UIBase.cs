using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public virtual void UIOpen()
    {
        gameObject.SetActive(true);
    }

    public virtual void UIClose()
    {
        gameObject.SetActive(false);
    }
}

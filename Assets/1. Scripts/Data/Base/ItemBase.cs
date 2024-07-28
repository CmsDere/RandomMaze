using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public virtual void Spawn()
    {
        gameObject.SetActive(true);
    }

    public virtual void Despawn()
    {
        gameObject.SetActive(false);
    }
}

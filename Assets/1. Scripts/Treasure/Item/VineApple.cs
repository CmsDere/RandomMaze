using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineApple : ItemBase
{
    void Awake()
    {
        DataManager.instance.LoadAll();
    }

    public override void Spawn()
    {
        base.Spawn();
    }

    public override void Despawn()
    {
        base.Despawn();
    }
}

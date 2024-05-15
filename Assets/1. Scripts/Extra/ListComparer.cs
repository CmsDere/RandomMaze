using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListComparer : IEqualityComparer<(Vector3 pos, int index)>
{
    public bool Equals((Vector3 pos, int index) x, (Vector3 pos, int index) y)
    {
        return x.pos.Equals(y.pos);
    }

    public int GetHashCode((Vector3 pos, int index) obj)
    {
        return obj.pos.GetHashCode();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DIRECTIONS
{
    NORTH,
    SOUTH,
    WEST,
    EAST
}

public class Wall : MonoBehaviour
{
    [SerializeField] int stage;
    [SerializeField] DIRECTION dir;
    [SerializeField] Vector3 wallPos;

    public int GetStage()
    {
        return stage;
    }

    public DIRECTION GetDIRECTION()
    {
        return dir;
    }

    public Vector3 GetPosition()
    {
        return wallPos;
    }

    public void SetWallData(DIRECTION _dir, Vector3 _wallPos, int _stage)
    {
        dir = _dir;
        wallPos = _wallPos;
        stage = _stage;
    }
}

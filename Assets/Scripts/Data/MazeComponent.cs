using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TRAP_TYPE
{
    NONE = -1,
    STONE_TRAP,
    ARROW_TRAP,
    SWAMP_TRAP,
    FALME_TRAP,
    MAX
}

public class MazeComponent : MonoBehaviour
{
    [Header("미로 생성 관련 중요 변수")]
    [SerializeField] protected int mazeWidth = 20;
    [SerializeField] protected int mazeHeight = 20; 
    [SerializeField] protected int stageLength = 3;

    protected bool IsInRange(int x, int z)
    {
        return x >= 0 && z >= 0 && x < mazeWidth && z < mazeHeight;
    }
}

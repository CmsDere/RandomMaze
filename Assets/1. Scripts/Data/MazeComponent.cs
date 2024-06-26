using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TRAP_TYPE
{
    STONE_TRAP,
    ARROW_TRAP,
    SWAMP_TRAP,
    FLAME_TRAP,
    MAX
}

public enum DIRECTION
{
    NORTH,
    SOUTH,
    WEST,
    EAST,
    MAX
}

public enum ITEM_TYPE
{
    EQUIP,
    FOOD,
    BUFF_TIMER,
    BUFF_CONST
}

public enum BUFF_TYPE
{
    SPEED,
    HEALTH
}

public class MazeComponent : MonoBehaviour
{
    [Header("미로 생성 관련 중요 변수")]
    [SerializeField] protected int mazeWidth = 20;
    [SerializeField] protected int mazeHeight = 20; 
    [SerializeField] protected int stageLength = 3;
    [SerializeField] protected int maxShortcutCount = 20;

    [Header("함정 생성 정보")]
    [SerializeField] protected int trapAmount = 20;
    [SerializeField] protected int stoneTrapRange = 4;
    [SerializeField] protected int maxRunwayLength = 4;

    [Header("함정 생성 개수")]
    [SerializeField] protected int stoneTrapAmount = 10;
    [SerializeField] protected int arrowTrapAmount = 40;
    [SerializeField] protected int swampTrapAmount = 40;
    [SerializeField] protected int flameTrapAmount = 40;

    [Header("보물 생성 개수")]
    [SerializeField] protected int treasureAmount = 20;

    protected string generateShortcutError = "지름길을 생성할 수 없는 벽입니다.";

    protected bool IsInRange(int x, int z)
    {
        return x >= 0 && z >= 0 && x < mazeWidth && z < mazeHeight;
    }
}

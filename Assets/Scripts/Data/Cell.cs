using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CELL_TYPE
{
    NONE,
    START,
    FINISH,
    TRAP,
    TREASURE    
}

public class Cell : MonoBehaviour 
{
    [SerializeField] int stage; // 스테이지 번호
    [SerializeField] int cell; // 지점 번호
    [SerializeField] Vector3 cellPos; // 지점 위치(vector.tostring)
    [SerializeField] CELL_TYPE cellType;

    public int GetStage()
    {
        return stage;
    }

    public void SetCellData(int _stage, int _cell, Vector3 _cellPos)
    {
        stage = _stage;
        cell = _cell;
        cellPos = _cellPos;
    }

    public void SetCellType(CELL_TYPE _cellType = CELL_TYPE.NONE)
    {
        cellType = _cellType;
    }
}

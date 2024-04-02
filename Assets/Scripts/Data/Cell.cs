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
    [SerializeField] int stage; // �������� ��ȣ
    [SerializeField] int cell; // ���� ��ȣ
    [SerializeField] Vector3 cellPos; // ���� ��ġ(vector.tostring)
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

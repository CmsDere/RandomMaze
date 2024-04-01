using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeData : MonoBehaviour 
{
    public int stage; // �������� ��ȣ
    public int cell; // ���� ��ȣ
    public Vector3 cellPos; // ���� ��ġ(vector.tostring)
    public List<(string, Vector3)> wall; // �� ����, �� ��ġ
    
    public MazeData(int stage, int cell, Vector3 cellPos, List<(string, Vector3)> wall)
    {
        this.stage = stage;
        this.cell = cell;
        this.cellPos = cellPos;
        this.wall = wall;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeData : MonoBehaviour 
{
    public int stage; // 스테이지 번호
    public int cell; // 지점 번호
    public Vector3 cellPos; // 지점 위치(vector.tostring)
    public List<(string, Vector3)> wall; // 벽 방향, 벽 위치
    
    public MazeData(int stage, int cell, Vector3 cellPos, List<(string, Vector3)> wall)
    {
        this.stage = stage;
        this.cell = cell;
        this.cellPos = cellPos;
        this.wall = wall;
    }
}

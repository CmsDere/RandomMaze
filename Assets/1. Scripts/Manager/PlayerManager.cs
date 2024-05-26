using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    MazeInformation mazeInfo;

    GameObject player;

    void Awake()
    {
        mazeInfo = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();   
    }

    public void GeneratePlayer()
    {
        player = Instantiate(playerPrefab, mazeInfo.mazeStartPos, Quaternion.identity);
        player.tag = "Player";
        GameManager.i.isPlayerSpawn = true;
        Cursor.visible = false;
    }
}

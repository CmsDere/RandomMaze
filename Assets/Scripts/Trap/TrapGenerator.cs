using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    [SerializeField] GameObject stoneTrapPrefab;

    MazeInformation mI;

    GameObject[] trapBaseObjects;
    GameObject[] stoneTrapObjects;
    int availableStoneTrap;

    List<int> randomRunwayIist = new List<int>();

    public float cellPosX { get; set; }
    public float cellPosY { get; set; }
    public float cellPosZ { get; set; }

    void Awake()
    {
        stoneTrapObjects = new GameObject[stageLength * stoneTrapAmount];
        trapBaseObjects = new GameObject[(int)TRAP_TYPE.MAX];
        mI = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();    
    }

    // 함정 최종 생성==
    // 돌 함정 최종 생성
    public void GenerateStoneTrap()
    {
        availableStoneTrap = mI.runways.Count;

        for (int i = 0; i < stageLength * stoneTrapAmount; i++)
        {
            int r = Random.Range(0, availableStoneTrap);
            randomRunwayIist.Add(r);
            if (randomRunwayIist.Count != randomRunwayIist.Distinct().Count())
            {
                randomRunwayIist = randomRunwayIist.Distinct().ToList();
            }
        }

        for (int i = 0; i < randomRunwayIist.Count; i++)
        {
            CreateStoneTrap(randomRunwayIist[i], i);
        }
    }

    // 화살 함정 최종 생성
    public void GenerateArrowTrap()
    {

    }
    //==

    //돌 함정 생성 관련==
    void CreateStoneTrap(int runwayIndex, int stoneTrapIndex)
    {
        Vector3 start = mI.runways[runwayIndex].start;
        Vector3 end = mI.runways[runwayIndex].end;
        string direction = mI.runways[runwayIndex].direction;
        Vector3 center = (end + start) / 2;

        stoneTrapObjects[stoneTrapIndex] = Instantiate
            (
                stoneTrapPrefab,
                transform.TransformDirection(new Vector3((end.x + start.x) / 2, end.y, (end.z + start.z) / 2)),
                Quaternion.identity
            );
        BoxCollider box = stoneTrapObjects[stoneTrapIndex].GetComponent<BoxCollider>();

        box.size = new Vector3(stoneTrapSizeX(end.x, start.x, direction), 1, stoneTrapSizeZ(end.z, start.z, direction));
        stoneTrapObjects[stoneTrapIndex].name = $"StoneTrap {stoneTrapIndex}";
        stoneTrapObjects[stoneTrapIndex].transform.parent = trapBaseObjects[(int)TRAP_TYPE.STONE_TRAP].transform;
        stoneTrapObjects[stoneTrapIndex].GetComponent<StoneTrap>().start = start;
        stoneTrapObjects[stoneTrapIndex].GetComponent<StoneTrap>().end = end;
        stoneTrapObjects[stoneTrapIndex].GetComponent<StoneTrap>().direction = direction;

        //Debug.Log($"TrapNum : {stoneTrapIndex}, Direction : {direction}, Start : {start}, End : {end}, Center : {center}");
    }

    public void GenerateTrapBase()
    {
        for (int i = 0; i < (int)TRAP_TYPE.MAX; i++)
        {
            trapBaseObjects[i] = new GameObject($"{(TRAP_TYPE)i}");
            trapBaseObjects[i].transform.parent = this.transform;
        }
    }

    float stoneTrapSizeX(float end, float start, string direction)
    {
        return (direction == "Horizontal") ? (end - start + 1) : 1;
    }

    float stoneTrapSizeZ(float end, float start, string direction)
    {
        return (direction == "Horizontal") ? 1 : (end - start + 1);
    }
    //==
}

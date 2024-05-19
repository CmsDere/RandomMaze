using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;



public class TrapGenerator : MazeComponent
{
    [SerializeField] GameObject stoneTrapPrefab;
    [SerializeField] GameObject arrowTrapPrefab;
    [SerializeField] GameObject swampTrapPrefab;
    [SerializeField] GameObject flameTrapPrefab;

    MazeInformation mI;

    GameObject[] trapBaseObjects;
    GameObject[] stoneTrapObjects;
    public List<GameObject> arrowTrapList { get; private set; } = new List<GameObject>();
    GameObject arrowTrapObject;
    List<GameObject> swampTrapList = new List<GameObject>();
    GameObject swampTrapObject;
    List<GameObject> flameTrapList = new List<GameObject>();
    GameObject flameTrapObject;
    int availableStoneTrap;
    int availableArrowTrap;

    List<int> randomRunwayList = new List<int>();
    public List<int> randomArrowTrapList { get; private set; } = new List<int>();

    public float cellPosX { get; set; }
    public float cellPosY { get; set; }
    public float cellPosZ { get; set; }

    void Awake()
    {
        trapBaseObjects = new GameObject[(int)TRAP_TYPE.MAX];
        stoneTrapObjects = new GameObject[stageLength * stoneTrapAmount];
        mI = GameObject.Find("MazeInformation").GetComponent<MazeInformation>();    
    }

    // 함정 최종 생성===
    // 돌 함정 최종 생성
    public void GenerateStoneTrap()
    {
        availableStoneTrap = mI.runways.Count;

        for (int i = 0; i < stageLength * stoneTrapAmount; i++)
        {
            int r = Random.Range(0, availableStoneTrap);
            randomRunwayList.Add(r);
            if (randomRunwayList.Count != randomRunwayList.Distinct().Count())
            {
                randomRunwayList = randomRunwayList.Distinct().ToList();
            }
        }

        for (int i = 0; i < randomRunwayList.Count; i++)
        {
            CreateStoneTrap(randomRunwayList[i], i);
        }
    }
    //==

    // 화살 함정 최종 생성
    public void GenerateArrowTrap()
    {
        availableArrowTrap = mI.arrowCell.Count;
        for(int i = 0; i < stageLength * arrowTrapAmount; i++)
        {
            int r = Random.Range(0, availableArrowTrap);
            randomArrowTrapList.Add(r);
            if (randomArrowTrapList.Count != randomArrowTrapList.Distinct().Count())
            {
                randomArrowTrapList = randomArrowTrapList.Distinct().ToList();
            }          
        }

        for (int i = 0; i < randomArrowTrapList.Count; i++)
        {
            CreateArrowTrap(randomArrowTrapList[i], i);
        }

    }
    //==

    // 늪 함정 최종 생성
    public void GenerateSwampTrap()
    {
        for (int i = 0; i < mI.swampTrapList.Count; i++)
        {
            CreateSwampTrap(i);
        }
    }
    //==

    // 화염 함정 최종 생성
    public void GenerateFlameTrap()
    {
        for (int i = 0; i < mI.flameTrapList.Count; i++)
        {
            CreateFlameTrap(i);
        }
    }
    //==
    //===

    // 화염 함정 생성 관련
    void CreateFlameTrap(int id)
    {
        Vector3 pos = mI.flameTrapList[id];
        flameTrapObject = Instantiate(flameTrapPrefab, transform.TransformDirection(pos), Quaternion.identity);
        flameTrapObject.name = $"FlameTrap {id}";
        flameTrapObject.transform.parent = trapBaseObjects[(int)TRAP_TYPE.FLAME_TRAP].transform;
        flameTrapList.Add(flameTrapObject);
    }
    //==

    // 늪 함정 생성 관련
    void CreateSwampTrap(int id)
    {
        Vector3 pos = mI.swampTrapList[id];
        swampTrapObject = Instantiate(swampTrapPrefab, transform.TransformDirection(pos), Quaternion.identity);
        swampTrapObject.name = $"SwampTrap {id}";
        swampTrapObject.transform.parent = trapBaseObjects[(int)TRAP_TYPE.SWAMP_TRAP].transform;
        swampTrapList.Add(swampTrapObject);
    }
    //==

    // 화살 함정 생성 관련
    void CreateArrowTrap(int arrowIndex, int arrowTrapIndex)
    {
        Vector3 pos = mI.arrowCell[arrowIndex].pos;
        string direction = mI.arrowCell[arrowIndex].direction;
        arrowTrapObject = Instantiate(arrowTrapPrefab, transform.TransformDirection(pos), Quaternion.identity);
        arrowTrapObject.name = $"ArrowTrap {arrowTrapIndex}";
        arrowTrapObject.transform.parent = trapBaseObjects[(int)TRAP_TYPE.ARROW_TRAP].transform;
        arrowTrapObject.GetComponent<ArrowTrap>().arrowTrapPos = pos;
        arrowTrapObject.GetComponent<ArrowTrap>().arrowTrapDirection = direction;
        arrowTrapList.Add(arrowTrapObject);
    }
    //==

    // 돌 함정 생성 관련==
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

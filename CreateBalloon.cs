using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBalloon : MonoBehaviour
{
    // 스폰 조절용 bool 변수
    public bool enableSpawn = false;
    
    // 풍선, 큰 풍선 prefab
    public GameObject Balloon, BigBalloon;
    
    //풍선 관리용 부모 GameObject
    public GameObject Balloons, BigBalloons;

    // 풍선 스폰 개수 제한용 bool 변수
    public bool isControlSpawn = false;

    // 피버타임시 스폰함수 호출 제한용 bool변수
    public bool isFeverChanged = false;
    
    // 풍선 스폰 위치 조절용 변수
    public float posz = 20f;

    // 가용 포지션(Vector2) 저장용 배열
    ArrayList arr = new ArrayList();

    // 풍선 스폰 함수
    public void SpawnEnemy()
    {
        // enableSpawn bool 변수가 참일때(스폰 가능한 상황일때)
        if (enableSpawn)
        {
            float randomX = Random.Range(-20f, 20f);
            float randomY = Random.Range(-4f, 20f);
            float randomsize = Random.Range(1, 100);
            arr = new ArrayList();

            // 80퍼센트의 확률로
            if (1 <= randomsize && randomsize <= 80)
            {
                // 가용한 포지션 뽑아서
                Vector2 pos = RandomAvailablePosition(4);
                // 가용한 포지션이 있으면 생성
                if (pos != Vector2.zero)
                    Instantiate(Balloon, new Vector3(pos.x, pos.y, posz), Quaternion.identity,Balloons.transform);
                // 없다면 enableSpawn 을 꺼서 생성 함수 지속 호출을 제한
                else
                    enableSpawn = false;
            }
            
            // 20퍼센트의 확률로
            if (80 < randomsize && randomsize <= 100)
            {
                Vector2 pos = RandomAvailablePosition(6);

                //가용 포지션 있으면 생성
                if (pos != Vector2.zero)
                    Instantiate(BigBalloon, new Vector3(pos.x, pos.y, posz), Quaternion.identity, BigBalloons.transform);
                // 위와 동일
                else
                    enableSpawn = false;
            }
        }
    }

    // 가용한 포지션 중 랜덤하게 하나 뽑아서 반환하는 함수
    Vector2 RandomAvailablePosition(int size)
    {
        for (int i = 0; i < 36; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float randomX = i - 18f;
                float randomY = j;
                //isCreatalbe함수: 생성 가능한 포지션이라면 true, 아니면 false를 반환
                if (isCreateable(randomX, randomY, size))
                {
                    arr.Add(new Vector2(randomX, randomY));
                }
            }
        }

        int arrsize = arr.Count;
        int index = Random.Range(0, arrsize - 1);

        // 가용 가능한 포지션이 없으면 Vector2.zero를 반환
        if (arrsize == 0) return Vector2.zero;
        // 가용 가능한 포지션이 있다면 포지션 중 랜덤한 포지션 반환
        else return (Vector2)arr[index];
    }

    // 주기적 SpawnEnemy 호출 설정(2초 후부터 1.5초마다 한번씩)
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 2, 1.5f);
    }

    void Update()
    {
        // if (Balloons.transform.childCount + BigBalloons.transform.childCount < 20 && !isControlSpawn)
        // {
        //     isControlSpawn = true;
        //     enableSpawn = true;
        // }
        // else if (Balloons.transform.childCount + BigBalloons.transform.childCount >= 20 && isControlSpawn)
        // {
        //     isControlSpawn = false;
        //     enableSpawn = false;
        // }
        // 위에께 원본

        if (Balloons.transform.childCount + BigBalloons.transform.childCount < 20)
            enableSpawn = true;
        else if (Balloons.transform.childCount + BigBalloons.transform.childCount >= 20)
            enableSpawn = false;

        if (GameObject.Find("FeverManager").GetComponent<FeverTime>().fever && !isFeverChanged)
        {
            CancelInvoke("SpawnEnemy");
            InvokeRepeating("SpawnEnemy", 0, 0.5f);
            isFeverChanged = true;
        }
        else if (!GameObject.Find("FeverManager").GetComponent<FeverTime>().fever && isFeverChanged)
        {
            CancelInvoke("SpawnEnemy");
            InvokeRepeating("SpawnEnemy", 0, 1.5f);
            isFeverChanged = false;
        }
    }

    bool isCreateable(float x, float y, int size)
    {
        bool ret = true;
        Vector3 origin = new Vector3(x, y, posz);
        Transform[] smallChildren = Balloons.GetComponentsInChildren<Transform>();
        foreach (Transform child in smallChildren)
        {
            if (child.name == Balloons.name) continue;
            if (Vector3.Distance(origin, child.transform.position) < size + 2)
            {
                ret = false;
                break;
            }
        }

        Transform[] bigChildren = BigBalloons.GetComponentsInChildren<Transform>();
        foreach (Transform child in bigChildren)
        {
            if (child.name == BigBalloons.name) continue;
            if (Vector3.Distance(origin, child.transform.position) < size + 3)
            {
                ret = false;
                break;
            }
        }
        return ret;
    }

}
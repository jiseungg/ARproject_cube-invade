using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{

    //BigBalloon Material count
    public int hitcount = 0;

    // Effects prefab
    public GameObject hitEffectPrefab, popEffect;
    
    //Items prefabs
    public GameObject reditem, blueitem, yellowitem;
    
    // To get CreateBallonComponent
    public CreateBalloon createBalloon;

    // Big Balloon Materials
    public Material[] hitMaterials = new Material[4];

    // Pop,hit audio clips
    public AudioClip popsound, hitsound;

    // Start is called before the first frame update
    void Start()
    {
        createBalloon = GameObject.Find("BalloonManager").GetComponent<CreateBalloon>();
    }

    private void OnTriggerEnter(Collider other)

    {
        Vector3 pos = this.transform.position;
        // 볼과 충돌하고 본인이 bigballoon 일때
        if (other.transform.CompareTag("ball") && transform.CompareTag("BigBalloon")) 
        {
            // 자리가 비었으니 풍선 생성 인자 true로 변경
            if (!createBalloon.enableSpawn) createBalloon.enableSpawn = true;
            // 맞은 곳에 터짐 이펙트 뿌리기
            Destroy(Instantiate(hitEffectPrefab, pos, Quaternion.identity), 2f);
            
            // hitcount를 통한 메테리얼 조작
            hitcount = hitcount + 1;
            if (hitcount < 5)
            {
                transform.GetChild(0).GetComponent<Renderer>().material = hitMaterials[hitcount-1];
            } 
            // 5번 맞았을 시
            if (hitcount == 5)
            {
                //float randomX = Random.Range(-8f, 8f); 
                //float randomZ = Random.Range(32f, 42f);
                // GameObject enemy = (GameObject)Instantiate(BigCube, new Vector3(randomX, 1f, randomZ), Quaternion.identity);
                
                // 피버 계수 추가
                GameObject.Find("FeverManager").GetComponent<FeverTime>().g += 4;
                itemDrop();
                Destroy(Instantiate(popEffect, pos, Quaternion.identity), 2f);
                AudioSource.PlayClipAtPoint(popsound, new Vector3(0, 0, 0));
                Destroy(gameObject);
            }
            other.GetComponent<BallColide>().TunrOffTempo();
        }

        //공에 맞은게 직은 풍선일 시
        if (other.transform.CompareTag("ball") && transform.CompareTag("Balloon")) 
        {
            // 위와 동일
            if (!createBalloon.enableSpawn) createBalloon.enableSpawn = true;

            // hit: 때리는 효과(주황색) pop: 터지는 효과(연기)
            Destroy(Instantiate(hitEffectPrefab, pos, Quaternion.identity), 2f);
            Destroy(Instantiate(popEffect, pos, Quaternion.identity), 2f);

            //float randomX = Random.Range(-8f, 8f);
            //float randomZ = Random.Range(32f, 42f);
            // GameObject enemy = (GameObject)Instantiate(Cube, new Vector3(randomX, 1f, randomZ), Quaternion.identity);
            
            GameObject.Find("FeverManager").GetComponent<FeverTime>().g += 1;
            itemDrop();
            AudioSource.PlayClipAtPoint(popsound, new Vector3(0, 0, 0));
            other.GetComponent<BallColide>().TunrOffTempo();
            Destroy(gameObject);
        }
    }


    private void itemDrop()
    {
        Vector3 pos = this.transform.position;
        float randomitem = Random.Range(1, 100);
        bool isFever = GameObject.Find("FeverManager").GetComponent<FeverTime>().fever;
        if (!isFever)
        {
            if (1 <= randomitem && randomitem <= 20)
            {
                Instantiate(reditem, pos, Quaternion.identity);
            }
            else if (20 < randomitem && randomitem <= 40)
            {
                Instantiate(blueitem, pos, Quaternion.identity);
            }
        }
        else
        {
            if (1 <= randomitem && randomitem <= 40){
                Instantiate(yellowitem, pos, Quaternion.identity);
            }
        }
    }
}
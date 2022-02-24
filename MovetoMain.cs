using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovetoMain : MonoBehaviour
{
    bool isLoad = false;
    
    // 초반에 카메라 세팅 등 옮겨야 할 것들이 나오기 전에 씬 이동하지 않도록 시간 세팅용
    public float loading = 0f;
    
    // 충돌 이펙트 prefab
    public GameObject hitEffect;

    // 로딩 시간 3초 부여
    private void Start()
    {
        loading = 3f;
    }

    // 로딩 시간 줄이기
    private void Update()
    {            
        loading -= Time.deltaTime;
        if(!isLoad && loading <= 0)
        {
            isLoad = true;

        }
    }

    // 충돌 위치 판정하기 위해 Trigger -> Collision으로 변경
    private void OnCollisionEnter(Collision other)
    {
        // 이펙트가 나오고 씬을 이동해야 하므로 Invoke 사용
        if (loading <= 0f)
        {
            Invoke("MoveScene", 1f);
        }
        Vector3 pos = other.contacts[0].point;
        GameObject hiteffect = Instantiate(hitEffect, pos, Quaternion.identity);
    }

    public void MoveScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}

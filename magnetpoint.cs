using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magnetpoint : MonoBehaviour
{
    // 빨려드는 힘
    public float force = 70f;

    // 빨려드는 장소
    public GameObject point;

    // 아이템의 rigidbody(start 함수에서 초기화 해줌)
    Rigidbody rigid;

    private void Start() {
        rigid = this.transform.GetComponent<Rigidbody>();    
    }

    // point의 포지션의 방향으로 force를 가함
    void FixedUpdate()
    {
        rigid.AddForce((point.transform.position - transform.position) * force * Time.fixedDeltaTime);
    }
}
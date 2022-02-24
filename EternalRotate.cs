using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 빙빙 돌아가는 회전목마처럼
public class EternalRotate : MonoBehaviour
{
    float rotSpeed = 36f;

    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0));
    }
}

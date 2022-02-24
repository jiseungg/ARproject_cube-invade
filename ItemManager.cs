using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // 빨간 아이템을 먹었을 시 나오는 검정색 가림막 prefab
    public GameObject blackimage;
    // 아이템 획득 사운드
    public AudioClip getsound;

    private void OnTriggerEnter(Collider other)
    {
        // 아이템 먹자마자 소리 출력하기(뿅)
        AudioSource.PlayClipAtPoint(getsound, new Vector3(0, 0, 0));
        Destroy(other.gameObject);
        
        // 빨간 아이템일 시
        if (other.transform.CompareTag("reditem"))
        {
            RedEffect();
        }

        else if (other.transform.CompareTag("blueitem"))
        {
            BlueEffect();
        }

        else if (other.transform.CompareTag("yellowitem"))
        {
            YellowEffect();
        }
    }

    private void BlueEffect()
    {
        Debug.Log("Blue Item Effect Function Called");
    }

    private void RedEffect()
    {
        Debug.Log("Red Item Effect Function Called");
        Destroy(Instantiate(blackimage), 5f);
    }

    private void YellowEffect()
    {
        Debug.Log("Yellow Item Effect Function Called");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    // 현재 체력 점수
    public float healthPoint = 1.0f;
    public Text gauge;
    public Image yellowgauge;
    // 게임 시작시 hp 바 오르는거 보여주는 용도
    float healthInit = 0f;
    // 현재 게임 오버 판단용 bool 변수
    public bool gameover = false;

    // Update is called once per frame
    void Update()
    {
        // 아직 바가 안 찼다면 게이지를 채운다(시간에 따라)
        if (healthInit < 1)
        {
            healthInit += Time.deltaTime/1.5f;
            gauge.text = healthInit.ToString();
            yellowgauge.fillAmount = healthInit;
        }
        // 바가 다 찼다면 현재 healthPoint에 따라 hp를 갱신한다.
        // 만약 0보다 작거나 같아진다면 GameOver함수를 호출한다.
        if (healthInit >= 1)
        {
            gauge.text = healthPoint.ToString();
            yellowgauge.fillAmount = healthPoint;
            if(healthPoint<=0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        gameover = true;
        Debug.Log("Game Over Function Called");
    }
}

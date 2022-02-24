using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverTime : MonoBehaviour
{
    //현재 피버 상태 bool
    public bool fever = false;
    public float feverSpeed = 8f;
    // 게이지 점수
    public float g = 0f;
    // 만점 기준
    public float maxScore = 40f;
    public Text gaugeText;
    public Image redgauge;

    // Update is called once per frame
    void Update()
    {
        gaugeText.text = g.ToString();
        if (!fever)
        {
            redgauge.fillAmount = g / maxScore;
        }
        else if (fever)
        {
            g -= Time.deltaTime * feverSpeed;
            redgauge.fillAmount = g / maxScore;
        }


        //피버가 maxScore보다 높아질 시 fever bool 변수 true로 변환
        if (g >= maxScore)       
        {
            fever = true;
        }

        if (g <= 0)
        {
            fever = false;
            g = 0;
        }
    }
}

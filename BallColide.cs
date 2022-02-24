using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallColide : MonoBehaviour
{
    OpenCVForUnityExample.BallTracker ballTracker;
    float lefttime = 0f;
    bool isOffed = false;
    bool isMoved = false;

    // Start is called before the first frame update
    void Start()
    {
        ballTracker = GameObject.Find("Final").GetComponent<OpenCVForUnityExample.BallTracker>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(lefttime > 0 )
            lefttime -= Time.deltaTime;
        
        if(isOffed && lefttime <= 0)
            EnableColider();
    }


    public void TunrOffTempo()
    {
        // this.GetComponent<SphereCollider>().enabled = false;
        // 디버깅 끝나면 다시 켜야함(아래줄)
        // this.tag = "ballAfter";
        lefttime = 10f;
        isOffed = true;
    }


    void EnableColider()
    {
        // this.GetComponent<SphereCollider>().enabled = true;
        // 디버깅 끝나면 다시 켜야함 (아래줄)
        // this.tag = "ball";
        isOffed = false;
        isMoved = false;
    }


    void MoveToLast()
    {
        if(!isMoved){
            int idx = ballTracker.balls.IndexOf(this.gameObject);
            ballTracker.balls.RemoveAt(idx);
            ballTracker.balls.Add(this.gameObject);
            isMoved = true;
        }
    }


    void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Balloon" || collider.gameObject.tag == "BigBalloon")
            Debug.Log("BALLON collider DETECTED");
            
        if(collider.gameObject.tag == "BottomLine")
        {
            Debug.Log("BOTTOM COLLISION DETECTED");
            MoveToLast();
        }
    }
}

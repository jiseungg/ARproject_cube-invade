using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollide : MonoBehaviour
{
    //Warning! Currently, this Scripts isn't used.
    private void OnTriggerEnter(Collider other) {
    if (other.transform.CompareTag("ball") && transform.CompareTag("Wall"))
        {
            Debug.Log("Wall Collide");
            other.GetComponent<BallColide>().TunrOffTempo();
        }
    }
}

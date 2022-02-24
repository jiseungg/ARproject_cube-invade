using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    // Warning! Currently, This Scripts isn't using.
    public GameObject Cube;
    void SpawnCube()
    {
        float randomX = Random.Range(-0.5f, 0.5f); 
        GameObject enemy = (GameObject)Instantiate(Cube, new Vector3(randomX, 1.1f, 0f), Quaternion.identity); 
    }
}

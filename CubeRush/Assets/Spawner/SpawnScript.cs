using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public GameObject Obstacle;

    public void CreateObstacle()
    {
        Instantiate(Obstacle, transform.position, Quaternion.identity);
    } 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTest_FollowMouse : MonoBehaviour
{
    public float speed;
    
    void Update()
    {
        transform.position += Vector3.Scale((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position), new Vector3(1, 1, 0)).normalized * speed;
    }
}

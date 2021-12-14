using System.Collections;
using System.Collections.Generic;
using UnityEngine;

  
public class billboardCustom : MonoBehaviour
{
    public Camera cameraToLookAt;

    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = 0f;
        //v.y = 90.0f;
        v.z = -90.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMe : MonoBehaviour
{
    public GameObject Camera;
    public float distanceToCamera;
    public float x;
    public float y;
    public float z;

    // Start is called before the first frame update
    void Start()
    {
        distanceToCamera = new Vector3(Camera.transform.position.x - this.gameObject.transform.position.x, Camera.transform.position.y - this.gameObject.transform.position.y, Camera.transform.position.z - this.gameObject.transform.position.z).magnitude;
    }

    // Update is called once per frame
    void Update()
    {

        /*
        //Y-Position
        y = Camera.transform.position.y;
        //Z-Position
        z = Mathf.Cos(Camera.transform.rotation.eulerAngles.y) * distanceToCamera;
        //X-Position
        x = Mathf.Tan(Camera.transform.rotation.y) * z;

        this.gameObject.transform.position = new Vector3(Camera.transform.position.x + x, y, Camera.transform.position.z + z);
        //this.gameObject.transform.rotation.eulerAngles.y = new Vector3(0, -Camera.transform.rotation.eulerAngles.y, 0);
        */
        this.gameObject.transform.LookAt(Camera.transform);
    }
}

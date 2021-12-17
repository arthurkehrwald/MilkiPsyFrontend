using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulse : MonoBehaviour
{

    public float speed;
    public float startSize;
    public float size;

    private float scale;
    
    // Start is called before the first frame update
    private void Update()
    {
        scale = startSize + Mathf.PingPong(Time.time * speed, size);
        
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }





}

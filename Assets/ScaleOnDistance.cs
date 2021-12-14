using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnDistance : MonoBehaviour
{
    //public GameObject gameObject;
    public Transform target;
    public float distance;
    float modifier;

    public float maxSize;
    public float growFactor;
    public float waitTime;

    void Start()
    {
       // target = this.transform.parent.GetComponent<DirectionalIndicator>().DirectionalTarget;
        distance = Mathf.Abs((target.position - this.gameObject.transform.position).magnitude);
        StartCoroutine(Scale());
    }

    // Update is called once per frame
    void Update()
    {
        // distance = Mathf.Abs((target.position - this.gameObject.transform.position).magnitude);
        
        //this.gameObject.transform.localScale = new Vector3(distance, distance, distance);
    }

    IEnumerator Scale()
    {
        float timer = 0;

        while (true) // this could also be a condition indicating "alive or dead"
        {
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (maxSize > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                //modifier = Time.deltaTime * growFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                //modifier = Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }
}

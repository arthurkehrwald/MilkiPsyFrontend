using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertColourOnStart1 : MonoBehaviour
{
    public double margin = 0.1;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        double currentDistance = 0.0;
        double maxDistance = 0.0;
        

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];

        /* The Original Code
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.blue, Color.green, vertices[i].x);
        */

        for (int i = 0; i < vertices.Length; i++)
        {
            currentDistance = 0.0 + Mathf.Abs(vertices[i].x);

            if (currentDistance > maxDistance) 
                maxDistance = currentDistance;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].x < -margin){
                colors[i] =  Color.blue;
                colors[i].a = (float)(1 * (Mathf.Abs(vertices[i].x) / maxDistance));
            }
            else if (vertices[i].x > margin)
            {
                colors[i] = Color.red;
                //colors[i].a = (float)(0.5 + 0.5 / (maxDistance / Mathf.Abs(vertices[i].x)));
                colors[i].a = (float)(1 * (Mathf.Abs(vertices[i].x) / maxDistance));
            }
            else
            {
                colors[i] = Color.white;
                colors[i].a = 0;
            }
                
        }
            


        // assign the array of colors to the Mesh.
        mesh.colors = colors;
    }
}

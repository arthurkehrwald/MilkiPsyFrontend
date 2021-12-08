using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertColourOnStart : MonoBehaviour
{
    public double margin = 0.1;
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        

        // create new colors array where the colors will be created.
        Color[] colors = new Color[vertices.Length];
       
        /* The Original Code
        for (int i = 0; i < vertices.Length; i++)
            colors[i] = Color.Lerp(Color.blue, Color.green, vertices[i].x);
        */


        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].x < -margin){
                colors[i] =  Color.blue;
            }
            else if (vertices[i].x > margin)
            {
                colors[i] = Color.red;
            }
            else
            {
                colors[i].a = 0;
            }
                
        }
            


        // assign the array of colors to the Mesh.
        mesh.colors = colors;
    }
}

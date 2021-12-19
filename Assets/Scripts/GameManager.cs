using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Program testProgram = new Program("example_program.json");
        testProgram.Start();
    }

}

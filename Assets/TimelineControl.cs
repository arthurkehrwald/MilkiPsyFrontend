using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineControl : MonoBehaviour
{   
    //Variables about the number of Steps
    public int numberOfSteps = 3;
    int currentStep = 0;

    //Timeline Variales
    public float lengthOfTimeline = 10;

    //Variables for managing the look of the Steps
    public float margin;
    public float sizeX = 1;
    public float sizeY = 1;
    public float sizeZ = 1;
    public GameObject Step;
    GameObject[] Steps;

    //Materials
    public Material StepFinished;
    public Material StepUnfinished;

    // Start is called before the first frame update
    void Start()
    {
        if ((numberOfSteps - 1) * margin < lengthOfTimeline)
            sizeX = (lengthOfTimeline - margin * (numberOfSteps - 1)) / (float)numberOfSteps;
        else
            Debug.LogWarning(this.gameObject.name + " has a too large margin.");

        Steps = new GameObject[numberOfSteps];
        //Instantiate all Step Objects, give them a proper Material and position and Scale everything
        for (int i = 0; i < Steps.Length; i++)
        {
            Steps[i] = Instantiate(Step, this.gameObject.transform.position + new Vector3(sizeX / 2 + i*(sizeX + margin),0,0), this.gameObject.transform.rotation);
            Steps[i].transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
            Steps[i].name = "Step" + i.ToString();
            Steps[i].GetComponent<MeshRenderer>().material = StepUnfinished;
            Steps[i].transform.SetParent(this.gameObject.transform);
        }
    }

    //Navigate through the steps and change Materials to reflect currentStep. current Step values are clamped.
    public void SetState(int a)
    {
        int finalValue = currentStep + a;
        if (finalValue <= numberOfSteps && finalValue >= 0)
        {
            currentStep = finalValue;
        } else
        {
            return;
        }

        for (int i = 0; i < finalValue; i++)
        {
            Steps[i].GetComponent<MeshRenderer>().material = StepFinished;
        }

        for (int i = finalValue; i < Steps.Length; i++)
        {
            Steps[i].GetComponent<MeshRenderer>().material = StepUnfinished;
        }
    }
}

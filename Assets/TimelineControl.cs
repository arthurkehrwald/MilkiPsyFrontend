using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineControl : MonoBehaviour
{   
    //Variables about the number of Steps
    public int numberOfSteps = 3;
    int currentStep = 0;

    //Timeline Variables
    public float lengthOfTimeline = 10;

    //Variables for managing the look of the Steps
    public float margin;
    public float sizeX = 1;
    public float sizeY = 1;
    public float sizeZ = 1;
    private bool instantiationSuccessful = false;
    public GameObject Step;
    GameObject[] Steps;

    //Materials
    public Material StepFinished;
    public Material StepUnfinished;

    //GameObject References
    private GameObject counter;
    private GameObject stepsParent;

    
    void Start()
    {
        counter = GameObject.FindGameObjectWithTag("Timer");

        Setup();
    }

    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            Setup();
        }
    }


    //Navigate through the steps and change Materials to reflect currentStep. current Step values are clamped.
    public void SetState(int a)
    {
        int finalValue = currentStep + a;
        if (finalValue <= numberOfSteps && finalValue >= 0)
        {
            currentStep = finalValue;
            counter.GetComponent<Counter>().SetTimer(counter.GetComponent<Counter>().startTime);
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

    public void Setup()
    {
        //margin = (lengthOfTimeline / 4) / (numberOfSteps - 1);

        //Check whether Timeline Subdivisions are possible
        if ((numberOfSteps - 1) * margin < lengthOfTimeline)
        {
            //Destroy Existing Game Objects
            if (instantiationSuccessful)
            {
                for (int i = 0; i < Steps.Length; i++)
                {
                    Destroy(Steps[i]);
                }
                Destroy(stepsParent);
            }

            sizeX = (lengthOfTimeline - margin * (numberOfSteps - 1)) / (float)numberOfSteps;

            //Setup Array that will hold references to step objects
            Steps = new GameObject[numberOfSteps];

            //setup parent for the step objects
            stepsParent = new GameObject("stepsParent");

            //Instantiate all Step Objects, give them a proper Material and set up their transform relative to each other
            for (int i = 0; i < Steps.Length; i++)
            {
                Steps[i] = Instantiate(Step);
                Steps[i].transform.position = new Vector3(0, 0, 0) + new Vector3(sizeX / 2 + i * (sizeX + margin), 0, 0);
                Steps[i].transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
                Steps[i].name = "Step" + i.ToString();
                Steps[i].GetComponent<MeshRenderer>().material = StepUnfinished;
                //Parent Step to a common parent
                Steps[i].transform.SetParent(stepsParent.transform);
            }
            //Align stepsParent to the timeline and then parent it to it
            stepsParent.transform.position = this.gameObject.transform.position;
            stepsParent.transform.rotation = this.gameObject.transform.rotation;
            stepsParent.transform.SetParent(this.gameObject.transform);

            //confirm successful initial instantiation
            instantiationSuccessful = true;
            //set step to start at the beginning
            currentStep = 0;
        }
        else
            Debug.LogWarning("Setup failed. Check margins");
    }
}

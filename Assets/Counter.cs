using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    private GameObject timeline;
    public float startTime = 80;
    bool timeIsRunning = true;
    float seconds;
    float minutes;
    string secondsRemaining;
    string minutesRemaining;
        
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the text component.
        // Since we are using the base class type <TMP_Text> this component could be either a <TextMeshPro> or <TextMeshProUGUI> component.
        m_TextComponent = GetComponent<TMP_Text>();
        timeline = GameObject.FindGameObjectWithTag("Timeline");        
        SetTimer(startTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeIsRunning)
        {
            seconds -= Time.deltaTime;
            if (seconds < 0)
            {
                if (minutes > 0)
                    minutes -= 1.0f;
                else
                {
                    ToggleTimer();
                    timeline.GetComponent<TimelineControl>().SetState(1);
                    return;
                }
                seconds = 60;
            }
               

            // Change the text on the text component.
            if ((int)seconds > 9)
                secondsRemaining = ((int)seconds).ToString();
            else
                secondsRemaining = "0" + ((int)seconds).ToString();

            if ((int)minutes > 9)
                minutesRemaining = ((int)minutes).ToString();
            else
                minutesRemaining = "0" + ((int)minutes).ToString();

        }

        m_TextComponent.text = minutesRemaining + ":" + secondsRemaining;

    }

    public void ToggleTimer()
    {
        timeIsRunning = !timeIsRunning;
    }

    public void SetTimer(float time)
    {
        startTime = time;
        minutes = (startTime - startTime % 60) / 60;
        seconds = startTime % 60;
    }

   
}

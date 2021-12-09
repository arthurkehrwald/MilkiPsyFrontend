using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    public float secondsOverall = 80;
    int seconds;
        
    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the text component.
        // Since we are using the base class type <TMP_Text> this component could be either a <TextMeshPro> or <TextMeshProUGUI> component.
        m_TextComponent = GetComponent<TMP_Text>();

        
    }

    // Update is called once per frame
    void Update()
    {
        secondsOverall -= Time.deltaTime;
        if (secondsOverall < 0)
            secondsOverall = 59;
        // Change the text on the text component.
        if (secondsOverall < 10)
            m_TextComponent.text = "02:0" + ((int)secondsOverall).ToString();
        else
            m_TextComponent.text = "02:" + ((int)secondsOverall).ToString();
    }
}

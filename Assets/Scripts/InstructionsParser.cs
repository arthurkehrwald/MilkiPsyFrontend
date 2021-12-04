using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class InstructionsParser : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI instructionsText;

    private void Start()
    {
        Parse("example_instructions");
    }

    public void Parse(string jsonName)
    {
        string jsonPath = Application.streamingAssetsPath + "/Instructions/" + jsonName + ".json";

        if (File.Exists(jsonPath))
        {
            string jsonText = File.ReadAllText(jsonPath);
            Instructions instructions = JsonUtility.FromJson<Instructions>(jsonText);
            instructionsText.text = instructions.text;
        }
        else
        {
            instructionsText.text = "Requested asset '" + jsonPath + "not found!";
        }
    }

    [System.Serializable]
    public struct Instructions
    {
        public string text;
    }
}

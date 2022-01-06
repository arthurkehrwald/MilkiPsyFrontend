using UnityEngine;
using System;

public class ConfigPaths : Singleton<ConfigPaths>
{
    public string ConfigFolderPath { get; private set; }
    public string ProgramFolderPath { get; private set; }
    public string StageFolderPath { get; private set; }
    public string InstructionsAndFeedbackPath { get; private set; }
    public string MediaFolderPath { get; private set; }
    public string ImageFolderPath { get; private set; }
    public string VideoFolderPath { get; private set; }
    public string AudioFolderPath { get; private set; }
    public string ServerAddressPath { get; private set; }

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        ConfigFolderPath = "/storage/emulated/0/Documents/MilkiPsyConfiguration";
#else
        ConfigFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/MilkiPsyConfiguration";
        ConfigFolderPath = ConfigFolderPath.Replace(@"\", "/");
#endif
        ProgramFolderPath = ConfigFolderPath + "/Programs";
        StageFolderPath = ConfigFolderPath + "/Stages";
        InstructionsAndFeedbackPath = ConfigFolderPath + "/InstructionsAndFeedback";
        MediaFolderPath = ConfigFolderPath + "/Media";
        ImageFolderPath = MediaFolderPath + "/Images";
        VideoFolderPath = MediaFolderPath + "/Videos";
        AudioFolderPath = MediaFolderPath + "/Audio";
        ServerAddressPath = ConfigFolderPath + "/ServerAddress.json";
    }
}

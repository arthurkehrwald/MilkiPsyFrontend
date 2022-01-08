using System;
using System.IO;

public static class ConfigPaths
{    
#if UNITY_ANDROID && !UNITY_EDITOR
    public static readonly string configFolderPath = "/storage/emulated/0/Documents/MilkiPsyConfiguration";
#else
    public static readonly string configFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MilkiPsyConfiguration");
#endif
    public static readonly string programFolderPath = Path.Combine(configFolderPath, "Programs");
    public static readonly string stageFolderPath = Path.Combine(configFolderPath, "Stages");
    public static readonly string instructionsAndFeedbackPath = Path.Combine(configFolderPath, "InstructionsAndFeedback");
    public static readonly string mediaFolderPath = Path.Combine(configFolderPath, "Media");
    public static readonly string imageFolderPath = Path.Combine(mediaFolderPath, "Images");
    public static readonly string videoFolderPath = Path.Combine(mediaFolderPath, "Videos");
    public static readonly string audioFolderPath = Path.Combine(mediaFolderPath, "Audio");
    public static readonly string serverAddressPath = Path.Combine(configFolderPath, "ServerAddress.json");
    public static readonly string loggingSettingsPath = Path.Combine(configFolderPath, "LoggingSettings.json");
}

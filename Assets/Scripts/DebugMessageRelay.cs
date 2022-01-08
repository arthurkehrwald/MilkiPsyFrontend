using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum DebugMessageType { Info, Error };
public class DebugMessageRelay : Singleton<DebugMessageRelay>
{
    public const string ReadError = "Failed to read or parse file '{0}'";

    private HashSet<IDebugMessageHandler> handlers = new HashSet<IDebugMessageHandler>();
    private LoggingSettings settings;

    public void RegisterHandler(IDebugMessageHandler handler)
    {
        handlers.Add(handler);
    }

    public void UnregisterHandler(IDebugMessageHandler handler)
    {
        handlers.Remove(handler);
    }

    public void RelayMessage(string message, DebugMessageType type)
    {
        foreach (IDebugMessageHandler handler in handlers)
        {
            handler.HandleDebugMessage(message, type);
        }
    }

    private void Start()
    {
        settings = ParseLoggingSettings();
    }

    private LoggingSettings ParseLoggingSettings()
    {
        string path = ConfigPaths.loggingSettingsPath;

        try
        {
            string json = FileAccessHelper.ReadText(path);
            LoggingSettings settings = JsonUtility.FromJson<LoggingSettings>(json);

            if (!settings.IsValid())
            {
                throw new Exception();
            }

            return settings;
        }
        catch (Exception)
        {
            string error = string.Format(ReadError, path);
            RelayMessage(error, DebugMessageType.Error);
            return default;
        }
    }

    [Serializable]
    private struct LoggingSettings : IParseResult
    {
        public bool logDebugInfo;
        public bool logDebugErrors;

        public bool IsValid()
        {
            return true;
        }
    }
}

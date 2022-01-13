using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public enum MediaType { Unknown, Invalid, Image, Video, Audio };

[Serializable]
public class InstructionsOrFeedback : IParseResult
{
    public string mediaFilename ;
    public string text;
    public string MediaFilePath
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(mediaFilePath))
            {
                return mediaFilePath;
            }

            mediaFilePath = MediaType switch
            {
                MediaType.Image => Path.Combine(ConfigPaths.imageFolderPath, mediaFilename),
                MediaType.Video => Path.Combine(ConfigPaths.videoFolderPath, mediaFilename),
                MediaType.Audio => Path.Combine(ConfigPaths.audioFolderPath, mediaFilename),
                _ => null,
            };

            return mediaFilePath;
        }
    }

    public MediaType MediaType
    {
        get
        {
            if (mediaType != MediaType.Unknown)
            {
                return mediaType;
            }

            string fileExtension = Path.GetExtension(mediaFilename);

            if (fileExtension == null)
            {
                mediaType = MediaType.Invalid;
                return mediaType;
            }

            mediaTypeOfExtension.TryGetValue(fileExtension, out mediaType);

            if (mediaType == default)
            {
                mediaType = MediaType.Invalid;
            }

            return mediaType;
        }
    }

    private string mediaFilePath;
    private MediaType mediaType;
    private readonly Dictionary<string, MediaType> mediaTypeOfExtension = new Dictionary<string, MediaType>()
    {
        { ".png", MediaType.Image },
        { ".jpg", MediaType.Image },
        { ".jpeg", MediaType.Image },
        { ".webm", MediaType.Video },
        { ".mp4", MediaType.Video },
        { ".wav", MediaType.Audio },
        { ".mp3", MediaType.Audio },
        { ".ogg", MediaType.Audio }
    };

    public static InstructionsOrFeedback ParseFromJson(string jsonFilename)
    {
        string instructionsPath = Path.Combine(ConfigPaths.instructionsAndFeedbackPath, jsonFilename);
        InstructionsOrFeedback ret;

        try
        {
            string instructionsJsonText = FileAccessHelper.ReadText(instructionsPath);
            ret = JsonUtility.FromJson<InstructionsOrFeedback>(instructionsJsonText);

            if (!ret.IsValid())
            {
                throw new Exception();
            }
        }
        catch
        {
            string error = string.Format(DebugMessageRelay.FileError, instructionsPath);
            throw new Exception(error);
        }

        return ret;
    }

    public bool IsValid()
    {
        bool isMediaFilenameValid = false;
        bool isMediaFilenameSpecified = !string.IsNullOrWhiteSpace(mediaFilename);

        if (isMediaFilenameSpecified)
        {
            FileInfo mediaFileInfo = new FileInfo(MediaFilePath);
            isMediaFilenameValid = mediaFileInfo.Exists && MediaType != MediaType.Invalid;
        }

        if (isMediaFilenameSpecified && !isMediaFilenameValid)
        {
            return false;
        }

        bool isTextValid = !string.IsNullOrWhiteSpace(text);

        return isMediaFilenameValid || isTextValid;
    }
}
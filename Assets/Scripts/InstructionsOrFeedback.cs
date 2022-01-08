using System;
using System.IO;
using System.Collections.Generic;

public enum MediaType { Unknown, Invalid, Image, Video, Audio };

[Serializable]
public class InstructionsOrFeedback : IParseResult
{
    public string mediaFileName;
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
                MediaType.Image => Path.Combine(ConfigPaths.imageFolderPath, mediaFileName),
                MediaType.Video => Path.Combine(ConfigPaths.videoFolderPath, mediaFileName),
                MediaType.Audio => Path.Combine(ConfigPaths.audioFolderPath, mediaFileName),
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

            string fileExtension = Path.GetExtension(mediaFileName);

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

    public bool IsValid()
    {
        bool isMediaFileNameValid = false;
        bool isMediaFileNameSpecified = !string.IsNullOrWhiteSpace(mediaFileName);

        if (isMediaFileNameSpecified)
        {
            FileInfo mediaFileInfo = new FileInfo(MediaFilePath);
            isMediaFileNameValid = mediaFileInfo.Exists && MediaType != MediaType.Invalid;
        }

        if (isMediaFileNameSpecified && !isMediaFileNameValid)
        {
            return false;
        }

        bool isTextValid = !string.IsNullOrWhiteSpace(text);

        return isMediaFileNameValid || isTextValid;
    }
}
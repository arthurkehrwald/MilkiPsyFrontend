using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using TMPro;

public class InstructionsOrFeedbackDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject textArea;
    [SerializeField]
    private GameObject mediaArea;
    [SerializeField]
    private AspectRatioFitter aspectRatioFitter;
    [SerializeField]
    private TextMeshProUGUI textUI;
    [SerializeField]
    private RawImage image;
    [SerializeField]
    private RawImage videoImage;
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private AudioSource audioSource;

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

    private readonly Dictionary<MediaType, string> folderPathOfMediaType = new Dictionary<MediaType, string>()
    {
        { MediaType.Image, Application.streamingAssetsPath + "/Configuration/Media/Images" },
        { MediaType.Video, Application.streamingAssetsPath + "/Configuration/Media/Videos" },
        { MediaType.Audio, Application.streamingAssetsPath + "/Configuration/Media/Audio" }
    };

    private enum MediaType { Invalid, Image, Video, Audio };

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {
        string text = runningStage.Instructions.text;

        string mediaFileName = runningStage.Instructions.mediaFileName;
        string mediaFileExtension = Path.GetExtension(mediaFileName);
        mediaTypeOfExtension.TryGetValue(mediaFileExtension, out MediaType mediaType);
        if (mediaType == MediaType.Invalid)
        {
            return;
        }
        string mediaFilePath = folderPathOfMediaType[mediaType] + "/" + mediaFileName;

        Display(text, mediaFilePath);
    }

    public void Display(string text, string mediaFilePath)
    {
        DisplayText(text);
        DisplayMedia(mediaFilePath);
    }

    private void DisplayText(string text)
    {
        bool isTextValid = !string.IsNullOrWhiteSpace(text);
        textArea.SetActive(isTextValid);

        if (!isTextValid)
            return;

        textUI.text = text;
    }

    private void DisplayMedia(string mediaFilePath)
    {
        string mediaFileExtension = Path.GetExtension(mediaFilePath);
        mediaTypeOfExtension.TryGetValue(mediaFileExtension, out MediaType mediaType);
        bool isMediaFilePathValid = mediaType != MediaType.Invalid;
        mediaArea.SetActive(isMediaFilePathValid);

        if (!isMediaFilePathValid)
            return;

        switch (mediaType)
        {
            case MediaType.Image:
                image.gameObject.SetActive(true);
                videoPlayer.gameObject.SetActive(false);
                audioSource.gameObject.SetActive(false);

                FileAccessHelper.RequestTexture(mediaFilePath, (texture) =>
                {
                    image.texture = texture;
                    aspectRatioFitter.aspectRatio = (float)image.texture.width / image.texture.height;
                });
                break;
            case MediaType.Video:
                image.gameObject.SetActive(false);
                videoPlayer.gameObject.SetActive(true);
                audioSource.gameObject.SetActive(false);

                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                videoPlayer.url = mediaFilePath;
                videoPlayer.prepareCompleted += (VideoPlayer videoPlayer) =>
                {
                    int videoWidth = videoPlayer.texture.width;
                    int videoHeight = videoPlayer.texture.height;
                    videoPlayer.targetTexture = new RenderTexture(videoWidth, videoHeight, 0);
                    videoImage.texture = videoPlayer.targetTexture;
                    aspectRatioFitter.aspectRatio = (float)videoWidth / videoHeight;
                    videoPlayer.Play();
                };
                break;
            case MediaType.Audio:
                image.gameObject.SetActive(false);
                videoPlayer.gameObject.SetActive(false);
                audioSource.gameObject.SetActive(true);

                FileAccessHelper.RequestAudioClip(mediaFilePath, (AudioClip clip) =>
                {
                    audioSource.Stop();
                    audioSource.clip = clip;
                    audioSource.Play();
                });
                break;
        }
    }
}

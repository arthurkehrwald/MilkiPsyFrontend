using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
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

    private enum MediaType { Invalid, Image, Video, Audio };

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(OnRunningStageChanged);
    }

    private void OnRunningStageChanged(Stage runningStage)
    {
        string text = runningStage?.Instructions.text;
        string mediaFileName = runningStage?.Instructions.mediaFileName;
        DisplayText(text);
        DisplayMediaAsync(mediaFileName);
    }


    private void DisplayText(string text)
    {
        bool isTextValid = !string.IsNullOrWhiteSpace(text);
        textArea.SetActive(isTextValid);

        if (!isTextValid)
            return;

        textUI.text = text;
    }

    private async Task DisplayMediaAsync(string mediaFileName)
    {
        MediaType mediaType = MediaTypeOfFile(mediaFileName);

        mediaArea.SetActive(mediaType != MediaType.Invalid);
        image.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        audioSource.gameObject.SetActive(false);

        switch (mediaType)
        {
            case MediaType.Image:
                await DisplayImageAsync(mediaFileName);
                break;
            case MediaType.Video:
                await DisplayVideoAsync(mediaFileName);
                break;
            case MediaType.Audio:
                await DisplayAudioAsync(mediaFileName); 
                break;
        }
    }

    private MediaType MediaTypeOfFile(string path)
    {
        string mediaFileExtension = Path.GetExtension(path);
        mediaTypeOfExtension.TryGetValue(mediaFileExtension, out MediaType mediaType);
        return mediaType;
    }

    private async Task DisplayImageAsync(string imageFileName)
    {
        string imageFilePath = ConfigFolderPaths.Instance.ImageFolderPath + "/" + imageFileName;
        image.texture = await FileAccessHelper.LoadTextureAsync(imageFilePath);
        aspectRatioFitter.aspectRatio = (float)image.texture.width / image.texture.height;
        image.gameObject.SetActive(true);
    }

    private async Task DisplayVideoAsync(string videoFileName)
    {
        string videoFilePath = ConfigFolderPaths.Instance.VideoFolderPath + "/" + videoFileName;

#if UNITY_WSA && !UNITY_EDITOR
        // On HoloLens, VideoPlayer can't read from user storage (e.g. Documents),
        // even if the appropriate capability is declared in the manifest.
        // As a workaround, copy videos to streaming assets folder first.
        string copyDest = Application.streamingAssetsPath + "/" + videoFileName;
        await FileAccessHelper.CopyAsync(videoFilePath, copyDest);
        videoFilePath = copyDest;
#endif

        videoPlayer.gameObject.SetActive(true);
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.url = videoFilePath;

        while (!videoPlayer.isPrepared)
        {
            await Task.Yield();
        }

        int videoWidth = videoPlayer.texture.width;
        int videoHeight = videoPlayer.texture.height;
        videoPlayer.targetTexture = new RenderTexture(videoWidth, videoHeight, 0);
        videoImage.texture = videoPlayer.targetTexture;
        aspectRatioFitter.aspectRatio = (float)videoWidth / videoHeight;
        videoPlayer.Play();
    }

    private async Task DisplayAudioAsync(string audioFileName)
    {
        string audioFilePath = ConfigFolderPaths.Instance.AudioFolderPath + "/" + audioFileName;
        audioSource.Stop();
        audioSource.clip = await FileAccessHelper.LoadAudioClipAsync(audioFilePath);
        audioSource.gameObject.SetActive(true);
        audioSource.Play();
    }
}

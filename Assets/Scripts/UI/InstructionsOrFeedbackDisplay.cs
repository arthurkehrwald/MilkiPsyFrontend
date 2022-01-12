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
    private bool hideByDefault = false;

    [SerializeField]
    private ActiveNotifier visuals;
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

    private void Awake()
    {
        GameManager.Instance.runningStageChanged.AddListener(RunningStageChangedHandler);
        visuals.isActiveChanged.AddListener(VisualsActiveChangedHandler);
    }

    private async void VisualsActiveChangedHandler(bool visualsActive)
    {
        if (visualsActive)
        {
            await Display(GameManager.Instance.RunningProgram?.RunningStage?.Instructions);
        }
    }

    private async void RunningStageChangedHandler(Stage runningStage)
    {
        await Display(runningStage?.Instructions);
    }

    public async Task Display(InstructionsOrFeedback instructionsOrFeedback)
    {
        if (instructionsOrFeedback == null)
        {
            DisplayText(null);
            await DisplayMediaAsync(null, MediaType.Invalid);
        }
        else
        {
            DisplayText(instructionsOrFeedback.text);
            await DisplayMediaAsync(instructionsOrFeedback.MediaFilePath, instructionsOrFeedback.MediaType);
        }

        if (hideByDefault && !textArea.activeSelf && !mediaArea.activeSelf)
        {
            visuals.gameObject.SetActive(false);
        }        
    }

    private void DisplayText(string text)
    {
        bool isTextValid = !string.IsNullOrWhiteSpace(text);
        textArea.SetActive(isTextValid);

        if (!isTextValid)
            return;

        textUI.text = text;
    }

    private async Task DisplayMediaAsync(string mediaFilePath, MediaType mediaType)
    {
        bool isMediaValid = mediaType != MediaType.Unknown && mediaType != MediaType.Invalid;
        mediaArea.SetActive(isMediaValid);

        if (!isMediaValid)
        {
            return;
        }

        switch (mediaType)
        {
            case MediaType.Image:
                await DisplayImageAsync(mediaFilePath);
                break;
            case MediaType.Video:
                await DisplayVideoAsync(mediaFilePath);
                break;
            case MediaType.Audio:
                await DisplayAudioAsync(mediaFilePath); 
                break;
        }
    }

    private async Task DisplayImageAsync(string imageFilePath)
    {
        image.texture = await FileAccessHelper.LoadTextureAsync(imageFilePath);
        aspectRatioFitter.aspectRatio = (float)image.texture.width / image.texture.height;
        image.gameObject.SetActive(true);
        videoPlayer.gameObject.SetActive(false);
        audioSource.gameObject.SetActive(false);
    }

    private async Task DisplayVideoAsync(string videoFilePath)
    {
#if UNITY_WSA && !UNITY_EDITOR
        // On HoloLens, VideoPlayer can't read from user storage (e.g. Documents),
        // even if the appropriate capability is declared in the manifest.
        // As a workaround, copy videos to streaming assets folder first.
        string videoFileName = Path.GetFileName(videoFilePath);
        string copyDest = Path.Combine(Application.streamingAssetsPath, videoFileName);
        await FileAccessHelper.CopyAsync(videoFilePath, copyDest);
        videoFilePath = copyDest;
#endif

        image.gameObject.SetActive(false);
        audioSource.gameObject.SetActive(false);
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

    private async Task DisplayAudioAsync(string audioFilePath)
    {
        audioSource.Stop();
        audioSource.clip = await FileAccessHelper.LoadAudioClipAsync(audioFilePath);
        audioSource.gameObject.SetActive(true);
        image.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        audioSource.Play();
    }
}

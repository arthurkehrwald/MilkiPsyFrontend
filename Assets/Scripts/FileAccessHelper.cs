using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

static class FileAccessHelper
{
    private static readonly Dictionary<string, AudioType> audioTypeOfExension = new Dictionary<string, AudioType>()
    {
        { ".wav", AudioType.WAV },
        { ".mp3", AudioType.MPEG},
        { ".ogg", AudioType.OGGVORBIS}
    };

    public static async void RequestAudioClip(string path, Action<AudioClip> callback)
    {
        string extension = Path.GetExtension(path);
        audioTypeOfExension.TryGetValue(extension, out AudioType type);
        bool isAudioTypeValid = type != AudioType.UNKNOWN;

        if (!isAudioTypeValid)
            return;

        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, type);

        // Send the request and wait for a response
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.error}: {request.downloadHandler.text}");
            return;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
        callback(clip);
    }

    public static async void RequestTexture(string path, Action<Texture2D> callback)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);

        // Send the request and wait for a response
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.error}: {request.downloadHandler.text}");
            return;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        callback(texture);
    }

    public static async void RequestJsonText(string path, Action<string> callback)
    {
        using UnityWebRequest request = UnityWebRequest.Get(path);

        // Send the request and wait for a response
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.error}: {request.downloadHandler.text}");
            return;
        }

        string jsonContent = request.downloadHandler.text;
        callback(jsonContent);
    }

    public static async Task<string> RequestJsonText(string path)
    {
        using UnityWebRequest request = UnityWebRequest.Get(path);

        // Send the request and wait for a response
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.error}: {request.downloadHandler.text}");
            return null;
        }

        string jsonContent = request.downloadHandler.text;
        return jsonContent;
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

static class FileAccessHelper
{
    private static readonly Dictionary<string, AudioType> audioTypeOfExtension = new Dictionary<string, AudioType>()
    {
        { ".wav", AudioType.WAV },
        { ".mp3", AudioType.MPEG},
        { ".ogg", AudioType.OGGVORBIS}
    };

    public static async void RequestAudioClip(string path, Action<AudioClip> callback)
    {
        path = AddFilePrefixIfNeeded(path);
        string extension = Path.GetExtension(path);
        audioTypeOfExtension.TryGetValue(extension, out AudioType type);
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

    public static async Task<AudioClip> LoadAudioClipAsync(string path)
    {
        path = AddFilePrefixIfNeeded(path);
        string extension = Path.GetExtension(path);
        audioTypeOfExtension.TryGetValue(extension, out AudioType type);
        bool isAudioTypeValid = type != AudioType.UNKNOWN;

        if (!isAudioTypeValid)
            return null;

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
            return null;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
        return clip;
    }

    public static async void RequestTexture(string path, Action<Texture2D> callback)
    {
        path = AddFilePrefixIfNeeded(path);
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
    public static async Task<Texture2D> LoadTextureAsync(string path)
    {
        path = AddFilePrefixIfNeeded(path);
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
            return null;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        return texture;
    }

    public static async void RequestJsonText(string path, Action<string> callback)
    {
        path = AddFilePrefixIfNeeded(path);
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

    public static async Task<string> LoadTextAsync(string path)
    {
        path = AddFilePrefixIfNeeded(path);
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

        string text = request.downloadHandler.text;
        return text;
    }

    public static string ReadText(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        using StreamReader reader = fileInfo.OpenText();
        string text = reader.ReadToEnd();
        return text;
    }

    public static async Task CopyAsync(string sourcePath, string destPath)
    {
        sourcePath = AddFilePrefixIfNeeded(sourcePath);
        using UnityWebRequest request = UnityWebRequest.Get(sourcePath);

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

        byte[] data = request.downloadHandler.data;

        using (FileStream SourceStream = File.Open(destPath, FileMode.Create))
        {
            SourceStream.Seek(0, SeekOrigin.End);
            await SourceStream.WriteAsync(data, 0, data.Length);
        }
    }

    private static string AddFilePrefixIfNeeded(string path)
    {
        if (!path.StartsWith("file://"))
        {
            path = "file://" + path;
        }
        return path;
    }

}
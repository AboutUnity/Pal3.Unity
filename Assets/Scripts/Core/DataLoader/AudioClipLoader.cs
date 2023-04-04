﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Core.DataLoader
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class AudioClipLoader
    {
        /// <summary>
        /// Load Unity AudioClip from file.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <param name="audioType">Audio type</param>
        /// <param name="streamAudio">Create streaming AudioClip</param>
        /// <param name="onLoaded">AudioClip callback invoker when loaded</param>
        /// <returns>IEnumerator</returns>
        public static IEnumerator LoadAudioClipAsync(string filePath,
            AudioType audioType,
            bool streamAudio,
            Action<AudioClip> onLoaded)
        {
            if (filePath.StartsWith("/")) filePath = filePath[1..];

            string url = "file:///" + filePath;

            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"Failed to load {url} with error: {request.error}");
            }
            else
            {
                try
                {
                    // Stream audio to avoid loading the entire AudioClip into memory at once,
                    // which can cause frame drops and stuttering during gameplay.
                    ((DownloadHandlerAudioClip) request.downloadHandler).streamAudio = streamAudio;

                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
                    onLoaded?.Invoke(audioClip);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to get audio clip: {filePath} with exception: {ex}");
                }
            }
        }
    }
}
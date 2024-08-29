using System;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Global Stable Diffusion parameters configuration.
/// </summary>
[ExecuteInEditMode]
public class StableDiffusionConfiguration : MonoBehaviour
{
    [SerializeField] public SDSettings settings;

    [SerializeField] public string[] samplers = new string[]
    {
        "Euler a", "Euler", "LMS", "Heun", "DPM2", "DPM2 a", "DPM++ 2S a", "DPM++ 2M", "DPM++ SDE", "DPM fast", "DPM adaptive",
        "LMS Karras", "DPM2 Karras", "DPM2 a Karras", "DPM++ 2S a Karras", "DPM++ 2M Karras", "DPM++ SDE Karras", "DDIM", "PLMS"
    };

    [SerializeField] public string[] modelNames;

    /// <summary>
    /// Data structure that represents a Stable Diffusion model to help deserialize from JSON string.
    /// </summary>
    class Model
    {
        public string title;
        public string model_name;
        public string hash;
        public string sha256;
        public string filename;
        public string config;
    }

    /// <summary>
    /// Method called when the user click on List Model from the inspector.
    /// </summary>
    public void ListModels()
    {
        StartCoroutine(ListModelsAsync());
    }

    /// <summary>
    /// Get the list of available Stable Diffusion models.
    /// </summary>
    /// <returns></returns>
    IEnumerator ListModelsAsync()
    {
        var url = settings.StableDiffusionServerURL + settings.ModelsAPI;

        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        try
        {
            var ms = JsonConvert.DeserializeObject<Model[]>(request.downloadHandler.text);
            modelNames = ms.Select(m => m.model_name).ToArray();
        }
        catch (Exception)
        {
            Debug.Log(request.downloadHandler.text);
            Debug.Log("Server needs and API key authentication. Please check your settings!");
        }
    }

    /// <summary>
    /// Set a model to use by Stable Diffusion.
    /// </summary>
    /// <param name="modelName">Model to set</param>
    /// <returns></returns>
    public IEnumerator SetModelAsync(string modelName)
    {
        string url = settings.StableDiffusionServerURL + settings.OptionAPI;

        if (modelNames == null || modelNames.Length == 0)
            yield return ListModelsAsync();

        try
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var sd = new SDOption
                {
                    sd_model_checkpoint = modelName
                };
                
                var json = JsonConvert.SerializeObject(sd);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                streamReader.ReadToEnd();
            }
        }
        catch (WebException e)
        {
            Debug.Log("Error: " + e.Message);
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using Txt2Img.Util;
using UnityEngine.Networking;

/// <summary>
/// Component to help generate a UI Image or RawImage using Stable Diffusion.
/// </summary>
[ExecuteAlways]
public class StableDiffusionText2Image : StableDiffusionGenerator
{
    [SerializeField] private string guid;

    // Backing fields with SerializeField attribute to expose them in the Inspector
    [SerializeField] private string prompt;
    [SerializeField] private string negativePrompt;
    [SerializeField] private PromptTheme promptTheme;

    // Public properties to access and modify the private backing fields
    public string Prompt
    {
        get => prompt;
        set => prompt = value;
    }

    public string NegativePrompt
    {
        get => negativePrompt;
        set => negativePrompt = value;
    }

    public string GuidField
    {
        get => guid;
        set => guid = value;
    }

    public PromptTheme PromptTheme
    {
        get => promptTheme;
        set => promptTheme = value;
    }

    HttpWebRequest txt2ImgWebRequest;

    /// <summary>
    /// List of samplers to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] samplersList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.samplers;
        }
    }

    /// <summary>
    /// Actual sampler selected in the drop-down list
    /// </summary>
    [HideInInspector] public int selectedSampler = 0;

    public int width = 512;
    public int height = 512;
    public int steps = 90;
    public float cfgScale = 7;
    public long seed = -1;

    public long generatedSeed = -1;

    string filename = "";


    /// <summary>
    /// List of models to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] modelsList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.modelNames;
        }
    }

    /// <summary>
    /// Actual model selected in the drop-down list
    /// </summary>
    [HideInInspector] public int selectedModel = 0;


    /// <summary>
    /// On Awake, fill the properties with default values from the selected settings.
    /// </summary>
    void Awake()
    {
#if UNITY_EDITOR
        if (width < 0 || height < 0)
        {
            StableDiffusionConfiguration sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            if (sdc != null)
            {
                SDSettings settings = sdc.settings;
                if (settings != null)
                {
                    width = settings.width;
                    height = settings.height;
                    steps = settings.steps;
                    cfgScale = settings.cfgScale;
                    seed = settings.seed;
                    return;
                }
            }

            width = 512;
            height = 512;
            steps = 50;
            cfgScale = 7;
            seed = -1;
        }
#endif
    }


    void Update()
    {
#if UNITY_EDITOR
        // Clamp image dimensions values between 128 and 2048 pixels
        if (width < 128) width = 128;
        if (height < 128) height = 128;
        if (width > 2048) width = 2048;
        if (height > 2048) height = 2048;

        // If not setup already, generate a GUID (Global Unique Identifier)
        if (guid == "")
            guid = Guid.NewGuid().ToString();
#endif
    }

    // Internally keep tracking if we are currently generating (prevent re-entry)
    public bool generating = false;

    /// <summary>
    /// Callback function for the inspector Generate button.
    /// </summary>
    public void Generate()
    {
        // Start generation asynchronously
        if (!generating && !string.IsNullOrEmpty(prompt))
        {
            StartCoroutine(GenerateAsync());
        }
    }

    /// <summary>
    /// Setup the output path and filename for image generation
    /// </summary>
    void SetupFolders()
    {
        // Get the configuration settings
        if (sdc == null)
            sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();

        try
        {
            // Determine output path
            string root = Application.dataPath + sdc.settings.OutputFolder;
            if (root == "" || !Directory.Exists(root))
                root = Application.streamingAssetsPath;
            string mat = Path.Combine(root, "SDImages");
            filename = Path.Combine(mat, $"{promptTheme.ToString().ToLower()}_{prompt.Split(", ")[0]}.png");

            // If folders not already exists, create them
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            if (!Directory.Exists(mat))
                Directory.CreateDirectory(mat);

            // If the file already exists, delete it
            if (File.Exists(filename))
                File.Delete(filename);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }

    public IEnumerator GenerateAsync()
    {
        generating = true;

        SetupFolders();

        // Set the model parameters
        sdc.SetModelAsync(modelsList[selectedModel]);

        // Generate the image
        try
        {
            // Create the request body
            SDParamsInTxt2Img sd = new SDParamsInTxt2Img
            {
                prompt = prompt,
                negative_prompt = negativePrompt,
                steps = steps,
                cfg_scale = cfgScale,
                width = width,
                height = height,
                seed = seed,
                tiling = false,
                sampler_name = selectedSampler >= 0 && selectedSampler < samplersList.Length
                    ? samplersList[selectedSampler]
                    : null,
            };

            if (promptTheme == PromptTheme.Background)
            {
                sd.alwayson_scripts = new Dictionary<string, object>();
            }

            string json = JsonConvert.SerializeObject(sd);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest request =
                   new UnityWebRequest(sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                // Add auth-header to request
                if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
                {
                    string encodedCredentials =
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass));
                    request.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                }

                // Send the request and wait for the response
                request.SendWebRequest();
                while (!request.isDone || request.result == UnityWebRequest.Result.InProgress)
                {
                    Debug.Log("Waiting for the request to complete...");
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                    generating = false;
                    yield break;
                }

                // Decode the response as a JSON string
                string result = request.downloadHandler.text;

                // Deserialize the JSON string into a data structure
                SDResponseTxt2Img jsonResponse = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

                // If no image, there was probably an error so abort
                if (jsonResponse.images == null || jsonResponse.images.Length == 0)
                {
                    Debug.LogError(
                        "No image was returned by the server. This should not happen. Verify that the server is correctly set up.");
                    generating = false;
                    yield break;
                }

                // Decode the image from Base64 string into an array of bytes
                byte[] imageData = Convert.FromBase64String(promptTheme == PromptTheme.Background
                    ? jsonResponse.images[0]
                    : jsonResponse.images[1]);

                // Write it in the specified project output folder
                File.WriteAllBytes(filename, imageData);

                try
                {
                    // Read back the image into a texture
                    if (File.Exists(filename))
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageData);
                        texture.Apply();

                        LoadIntoImage(texture);
                    }

                    // Read the generation info back (only seed should have changed, as the generation picked a particular seed)
                    if (!string.IsNullOrEmpty(jsonResponse.info))
                    {
                        SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(jsonResponse.info);

                        // Read the seed that was used by Stable Diffusion to generate this result
                        generatedSeed = info.seed;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }

        generating = false;
    }

    void StartWebRequest()
    {
        txt2ImgWebRequest.BeginGetResponse(FinishWebRequest, null);
    }

    void FinishWebRequest(IAsyncResult webResult)
    {
        var response = txt2ImgWebRequest.EndGetResponse(webResult);


        using (var streamReader = new StreamReader(response.GetResponseStream()))
        {
            // Decode the response as a JSON string
            string result = streamReader.ReadToEnd();

            // Deserialize the JSON string into a data structure
            SDResponseTxt2Img json = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

            // If no image, there was probably an error so abort
            if (json.images == null || json.images.Length == 0)
            {
                Debug.LogError(
                    "No image was return by the server. This should not happen. Verify that the server is correctly setup.");

                generating = false;
                //yield break;
            }

            // Decode the image from Base64 string into an array of bytes
            byte[] imageData = Convert.FromBase64String(json.images[1]);

            // Write it in the specified project output folder
            using (FileStream imageFile = new FileStream(filename, FileMode.Create))
            {
                imageFile.WriteAsync(imageData, 0, imageData.Length);
            }

            try
            {
                // Read back the image into a texture
                if (File.Exists(filename))
                {
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    texture.Apply();

                    LoadIntoImage(texture);
                }

                // Read the generation info back (only seed should have changed, as the generation picked a particular seed)
                if (json.info != "")
                {
                    SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(json.info);

                    // Read the seed that was used by Stable Diffusion to generate this result
                    generatedSeed = info.seed;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n\n" + e.StackTrace);
            }
        }
    }

    /// <summary>
    /// Load the texture into an Image or RawImage.
    /// </summary>
    /// <param name="texture">Texture to setup</param>
    void LoadIntoImage(Texture2D texture)
    {
        try
        {
            // Convert the Texture2D to a Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            if (promptTheme == PromptTheme.Background)
            {
                Image image = GetComponent<Image>();

                if (image != null)
                {
                    image.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("SpriteRenderer component not found on the GameObject.");
                }
            }
            else
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("SpriteRenderer component not found on the GameObject.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }
}
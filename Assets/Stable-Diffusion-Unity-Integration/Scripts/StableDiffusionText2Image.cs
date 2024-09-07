using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine.UI;
using Txt2Img.Util;
using UnityEngine.Networking;

/// <summary>
/// Component to help generate a UI Image or RawImage using Stable Diffusion.
/// </summary>
[ExecuteAlways]
public class StableDiffusionText2Image : MonoBehaviour
{
    [SerializeField] private string guid;

    // Backing fields with SerializeField attribute to expose them in the Inspector
    [SerializeField] private string prompt;
    [SerializeField] private string negativePrompt;
    [SerializeField] private PromptTheme promptTheme;

    private static StableDiffusionConfiguration sdc = null;

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

    public int width = Constants.GeneratedSpriteWidth;
    public int height = Constants.GeneratedSpriteHeight;
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

            width = Constants.GeneratedSpriteWidth;
            height = Constants.GeneratedSpriteHeight;
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
    private void SetupFolders()
    {
        if (sdc == null)
            sdc = FindObjectOfType<StableDiffusionConfiguration>();
        try
        {
            var root = Application.streamingAssetsPath;
            var mat = Path.Combine(root, "SDImages");

            // Ensure the directories exist
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            if (!Directory.Exists(mat))
                Directory.CreateDirectory(mat);

            // Construct the initial file name
            string baseFilename = $"{promptTheme.ToString().ToLower()}_{prompt.Split(", ")[0]}";
            string filePath = Path.Combine(mat, $"{baseFilename}.png");
            string finalFilename = filePath;

            // Check if file exists, and if it does, append a number to the filename
            int counter = 1;
            while (File.Exists(finalFilename))
            {
                finalFilename = Path.Combine(mat, $"{baseFilename} ({counter}).png");
                counter++;
            }

            filename = finalFilename;
        }
        catch (Exception e)
        {
            Debug.LogError(string.Join("\n\n", e.Message, e.StackTrace));
        }
    }

    public IEnumerator GenerateAsync([CanBeNull] Action<int> loadingCallback = null)
    {
        generating = true;

        SetupFolders();

        sdc.SetModelAsync(modelsList[selectedModel]);

        var sd = new SDParamsInTxt2Img
        {
            prompt = prompt,
            negative_prompt = negativePrompt,
            steps = steps,
            cfg_scale = cfgScale,
            width = Constants.GeneratedSpriteWidth,
            height = Constants.GeneratedSpriteWidth,
            seed = seed,
            tiling = false,
            sampler_name = selectedSampler >= 0 && selectedSampler < samplersList.Length
                ? samplersList[selectedSampler]
                : null,
        };

        if (promptTheme == PromptTheme.Background)
        {
            sd.alwayson_scripts = new();
            sd.width = Constants.GeneratedBackgroundWidth;
            sd.height = Constants.GeneratedBackgroundHeight;
        }

        if (promptTheme is PromptTheme.UIBackground or PromptTheme.UIButton)
        {
            sd.alwayson_scripts = new();
            sd.tiling = true;
            if (promptTheme is PromptTheme.UIButton)
            {
                sd.width = Constants.GeneratedUiButtonWidth;
                sd.height = Constants.GeneratedUiButtonHeight;
            }
        }

        string json = JsonConvert.SerializeObject(sd);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using var request =
            new UnityWebRequest(sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI, "POST");

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.SendWebRequest();

        while (!request.isDone || request.result == UnityWebRequest.Result.InProgress)
        {
            var (imageData, percentage) = GetGenerationData();
            SaveAndLoadImage(Convert.FromBase64String(imageData ?? ""));
            loadingCallback?.Invoke((int)(percentage * 100));
            yield return 1.0f;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            generating = false;
            yield break;
        }

        string result = request.downloadHandler.text;

        SDResponseTxt2Img jsonResponse = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

        if (jsonResponse.images == null || jsonResponse.images.Length == 0)
        {
            Debug.LogError(
                "No image was returned by the server. This should not happen. Verify that the server is correctly set up.");
            generating = false;
            yield break;
        }

        if (!string.IsNullOrEmpty(jsonResponse.info))
        {
            SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(jsonResponse.info);

            generatedSeed = info.seed;
        }

        var shouldRetrieveTransparentImage = promptTheme is PromptTheme.Background or PromptTheme.UIBackground or PromptTheme.UIButton;
        byte[] finalImageData = Convert.FromBase64String(shouldRetrieveTransparentImage ? jsonResponse.images[0] : jsonResponse.images[1]);

        SaveAndLoadImage(finalImageData);

        generating = false;
    }

    private void SaveAndLoadImage(byte[] imageData)
    {
        if (imageData.Length == 0)
        {
            return;
        }
        
        try
        {
            File.WriteAllBytes(filename, imageData);

            if (!File.Exists(filename))
            {
                return;
            }

            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            texture.Apply();

            LoadIntoImage(texture);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Join("\n\n", e.Message, e.StackTrace));
        }
    }

    private static (string imageData, float percentage) GetGenerationData()
    {
        var url = sdc.settings.StableDiffusionServerURL + sdc.settings.ProgressAPI;

        using WebClient client = new WebClient();
        string responseBody = client.DownloadString(url);

        SDProgress sdp = JsonConvert.DeserializeObject<SDProgress>(responseBody);

        return (sdp.current_image, sdp.progress);
    }

    /// <summary>
    /// Load the texture into an Image or RawImage.
    /// </summary>
    /// <param name="texture">Texture to setup</param>
    private void LoadIntoImage(Texture2D texture)
    {
        try
        {
            // Convert the Texture2D to a Sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            if (GetComponent<Image>() != null)
            {
                Image image = GetComponent<Image>();

                if (image != null)
                {
                    image.sprite = sprite;
                }
            }
            else
            {
                Debug.LogWarning("Image component not found on the GameObject.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(string.Join("\n\n", e.Message, e.StackTrace));
        }
    }
}
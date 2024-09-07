using System;
using System.Collections.Generic;
using Txt2Img.ThemedTxt2Img;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class AIManager : MonoBehaviour
{
    public List<PromptResult> promptResultObjects;
    
    [FormerlySerializedAs("EditingPromptResult")] public PromptResult editingPromptResult;
    
    public bool isDiffusionInProgress;
    
    [FormerlySerializedAs("promptResultFilenames")] public  List<string> galleryResultFilenames = new();
    
    public readonly Dictionary<PromptTheme, Prompt> PromptResults = new();

    public static AIManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public (string prompt, PromptTheme promptTheme, string promptThemeString) ResolveAttributesFromFilename(string filename)
    {
        var filenameWithoutExtension = filename.Substring(0, filename.LastIndexOf(".", StringComparison.Ordinal));
        var prompt = filenameWithoutExtension.Split("_")[1];
        var filenameThemePart = filenameWithoutExtension.Split("_")[0];

        return (prompt, filenameThemePart.ToPromptTheme(), filenameThemePart.ToPromptThemeString());
    } 
}
using System.Collections.Generic;
using Txt2Img.ThemedTxt2Img;
using Txt2Img.Util;
using UnityEngine;

public sealed class AIManager : MonoBehaviour
{
    public List<PromptResult> promptResultObjects;
    
    public PromptResult EditingPromptResult;
    
    public readonly Dictionary<PromptTheme, Prompt> PromptResults = new();

    public static AIManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
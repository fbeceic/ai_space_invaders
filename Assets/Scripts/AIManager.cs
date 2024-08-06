using System.Collections.Generic;
using TMPro;
using Txt2Img.ThemedTxt2Img;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class AIManager : MonoBehaviour
{
    public Dictionary<PromptTheme, Prompt> PromptResults = new();

    public static AIManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
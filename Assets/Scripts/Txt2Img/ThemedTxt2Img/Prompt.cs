using JetBrains.Annotations;
using Txt2Img.Util;
using UnityEngine;

namespace Txt2Img.ThemedTxt2Img
{ 
    public class Prompt
    {
        public string Text { get; set; }
        public PromptTheme Theme { get; set; }
        [CanBeNull] public Sprite Result { get; set; }
        
        public bool IsExecuted => Result is not null;
    }
}
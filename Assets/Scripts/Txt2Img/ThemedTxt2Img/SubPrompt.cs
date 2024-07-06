using JetBrains.Annotations;
using UnityEngine;

namespace Txt2Img.ThemedTxt2Img
{ 
    public class SubPrompt
    {
        public string Text { get; set; }
        [CanBeNull] public Sprite Result { get; set; }
        
        public bool IsExecuted => Result is not null;
    }
}
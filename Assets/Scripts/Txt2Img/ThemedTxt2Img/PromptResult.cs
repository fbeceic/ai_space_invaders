using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{ 
    public class PromptResult: MonoBehaviour
    {
        [SerializeField] public PromptTheme promptTheme;

        private Text promptText;

        private Image promptImage;
        
        private void Start()
        {
            promptText = GetComponentInChildren<Text>();
            promptImage = GetComponentInChildren<Image>();
        }

        public void ApplyPromptFeatures()
        {
            
            
        }
    }
}
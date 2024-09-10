using System.IO;
using UnityEngine;

namespace Txt2Img.Util
{
    public static class Constants
    {
        public const int GeneratedSpriteWidth = 512;
        public const int GeneratedSpriteHeight = 512;
        
        public const int GeneratedUiButtonWidth = 512;
        public const int GeneratedUiButtonHeight = 256;
        
        public const int GeneratedBackgroundWidth = 1024;
        public const int GeneratedBackgroundHeight = 1024;

        public static readonly string DefaultElementPlaceholder = "DEFAULT ELEMENT"; 
        
        public static string GeneratedImagesOutputFolder = Path.Combine(Application.streamingAssetsPath, "SDImages");
    }
}
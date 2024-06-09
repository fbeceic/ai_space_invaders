using System.Collections.Generic;

namespace Txt2Img.ThemedTxt2Img
{
    public interface IThemedTxt2Img
    {
        void StartTxt2ImgGeneration(); 
        void GetInputSubPrompts();   
        void RunSubPrompts();
    }
}

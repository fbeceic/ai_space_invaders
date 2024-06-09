using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class ThemedTxt2Img : MonoBehaviour, IThemedTxt2Img
    {
        public List<StableDiffusionText2Image> diffusionGenerators;
        
        public List<TMP_InputField> inputFields;

        private List<SubPrompt> inputSubPrompts;

        // Start is called before the first frame update
        void Start()
        {
            inputFields = new List<TMP_InputField>(FindObjectsOfType<TMP_InputField>());

            SceneManager.LoadScene("Space Invaders", LoadSceneMode.Additive);
            SceneManager.sceneLoaded += DisableObjects;
        }
        
        private void DisableObjects(Scene scene, LoadSceneMode mode)
        {
            diffusionGenerators = new List<StableDiffusionText2Image>(FindObjectsOfType<StableDiffusionText2Image>());
            GameObject[] objectsInScene = scene.GetRootGameObjects();

            foreach (var obj in objectsInScene)
            {
                if (obj.GetComponent<StableDiffusionText2Image>() == null)
                {
                    obj.SetActive(false);
                }
            }
        }

        public void StartTxt2ImgGeneration()
        {
            GetInputSubPrompts();
            RunSubPrompts();
        }

        public void GetInputSubPrompts()
        {
            SceneManager.LoadScene("Space Invaders");
            List<SubPrompt> subPrompts = new();
            foreach (var inputField in inputFields)
            {
                    subPrompts.Add(new SubPrompt {Text = inputField.text});
            }

            inputSubPrompts = subPrompts;
        }

        public void RunSubPrompts()
        {
            foreach (var diffusionGenerator in diffusionGenerators) {

                foreach (var subPrompt in inputSubPrompts)
                {
                    // Set the prompt text
                    diffusionGenerator.prompt = subPrompt.Text;

                    // Generate the image
                    diffusionGenerator.prompt = subPrompt.Text;
                    diffusionGenerator.Generate();

                    // Wait for the generation to complete
                    while (diffusionGenerator.generating)
                    {
                        // You can add progress indication here if needed
                    }

                    // Set the result image
                    subPrompt.Result = diffusionGenerator.GetComponent<RawImage>().texture as Texture2D;
                }
            }
        }
    }
}
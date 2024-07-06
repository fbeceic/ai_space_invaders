using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class ThemedTxt2Img : MonoBehaviour, IThemedTxt2Img
    {
        public List<StableDiffusionText2Image> diffusionGenerators;

        public List<TMP_InputField> inputFields;

        private List<SubPrompt> inputSubPrompts;

        public void StartTxt2ImgGeneration()
        {
            inputFields = new List<TMP_InputField>(FindObjectsOfType<TMP_InputField>());
            inputFields.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.OrdinalIgnoreCase));
            diffusionGenerators = new List<StableDiffusionText2Image>(FindObjectsOfType<StableDiffusionText2Image>());

            GetInputSubPrompts();
            RunSubPrompts();
        }

        public void GetInputSubPrompts()
        {
            List<SubPrompt> subPrompts = new();
            foreach (var inputField in inputFields)
            {
                subPrompts.Add(new SubPrompt { Text = inputField.text });
            }

            inputSubPrompts = subPrompts;
        }

        public void RunSubPrompts()
        {
            // TODO: map this onto sprites
            foreach (var diffusionGenerator in diffusionGenerators)
            {
                foreach (var subPrompt in inputSubPrompts)
                {
                    diffusionGenerator.Prompt = subPrompt.Text;
                    diffusionGenerator.GuidField = Guid.NewGuid().ToString();

                    if (!diffusionGenerator.generating)
                    {
                        StartCoroutine(diffusionGenerator.GenerateAsync());
                    }

                    // Wait for the generation to complete
                    while (diffusionGenerator.generating)
                    {
                        // You can add progress indication here if needed
                    }

                    // Set the result image
                    subPrompt.Result = diffusionGenerator.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }

        private string ExtendSubPrompt(string subprompt, string type)
        {
            switch (type)
            {
                case "background":
                    return subprompt + ",";
                case "player":
                    return subprompt;
                case "enemy":
                    return subprompt;
                case "projectile":
                    return subprompt;
                case "shield":
                    return subprompt;
            }
        } 
    }
}
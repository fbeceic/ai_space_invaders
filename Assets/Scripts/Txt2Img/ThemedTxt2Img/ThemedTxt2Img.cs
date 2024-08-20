using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class ThemedTxt2Img : MonoBehaviour
    {
        public List<StableDiffusionText2Image> diffusionGenerators;

        public List<TMP_InputField> inputFields;

        [SerializeField] private List<Prompt> inputPrompts;

        // private void Awake()
        // {
        //     diffusionGenerators = Resources.FindObjectsOfTypeAll<StableDiffusionText2Image>()
        //         .Where(instance => instance.gameObject.scene.IsValid()).ToList();
        // }

        public void StartTxt2ImgGeneration()
        {
            inputFields.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.OrdinalIgnoreCase));
            SetInputPrompts();
            RunInputPrompts();
        }

        private void SetInputPrompts()
        {
            List<Prompt> prompts = new();
            foreach (var inputField in inputFields)
            {
                var fieldTheme = inputField.GetComponent<PromptThemedInput>().promptTheme;
                prompts.Add(new() { Text = inputField.text, Theme = fieldTheme });
            }

            inputPrompts = prompts;
        }

        private void RunInputPrompts()
        {
            MenuManager.Instance.ShowMenu(1);
            var promptResults = FindObjectsOfType<PromptResult>().ToList();

            StartCoroutine(ProcessInputPrompts(promptResults));
        }

        private IEnumerator ProcessInputPrompts(List<PromptResult> promptResults)
        {
            promptResults.ForEach(result => result.imageGameObject.SetActive(false));
            
            foreach (var diffusionGenerator in diffusionGenerators)
            {
                var matchingPrompt = inputPrompts.Find(input => input.Theme == diffusionGenerator.PromptTheme);
                var matchingPromptResult = promptResults.Find(result => result.theme == diffusionGenerator.PromptTheme);

                matchingPromptResult.imageGameObject.SetActive(false);
                PromptHelper.InvokeTxt2ImgGeneration(this, diffusionGenerator, matchingPrompt.Text,
                    matchingPrompt.Theme, matchingPromptResult.UpdateProgressBar);

                while (diffusionGenerator.generating)
                {
                    yield return null;
                }

                matchingPromptResult.loadingSpinner.SetActive(false);
                matchingPromptResult.imageGameObject.SetActive(true);
                
                matchingPromptResult.ApplyPromptFeatures(matchingPrompt.Text,
                    diffusionGenerator.gameObject.GetComponent<Image>().sprite);
                matchingPromptResult.SaveSpriteToAIManager();
            }
        }
    }
}
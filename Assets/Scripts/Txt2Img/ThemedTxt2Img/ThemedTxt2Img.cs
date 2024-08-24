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

        public void StartTxt2ImgGeneration()
        {
            inputFields.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.OrdinalIgnoreCase));
            SetInputPrompts();
            RunInputPrompts();
        }

        private void SetInputPrompts()
        {
            List<Prompt> prompts = new();

            inputFields.ForEach(inputField =>
            {
                var fieldThemes = inputField.GetComponents<PromptThemedInput>().ToList();
                fieldThemes.ForEach(fieldTheme => { prompts.Add(new() { Text = inputField.text, Theme = fieldTheme.promptTheme }); });
            });

            inputPrompts = prompts;
        }

        private void RunInputPrompts()
        {
            MenuManager.Instance.ShowMenu(1);
            var promptResults = AIManager.Instance.promptResultObjects.ToList();

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
                    diffusionGenerator.PromptTheme, matchingPromptResult.UpdateProgressBar);

                while (diffusionGenerator.generating)
                {
                    yield return null;
                }

                matchingPromptResult.loadingSpinner.SetActive(false);
                matchingPromptResult.imageGameObject.SetActive(true);

                matchingPromptResult.ApplyPromptLabel(matchingPrompt.Text);
                matchingPromptResult.SaveSpriteToAIManager();
                if (matchingPromptResult.theme is PromptTheme.UIBackground)
                {
                    matchingPromptResult.ApplyUIBackgrounds();
                }
                if (matchingPromptResult.theme is PromptTheme.UIButton)
                {
                    matchingPromptResult.ApplyUIButtons();
                }
            }
        }
    }
}
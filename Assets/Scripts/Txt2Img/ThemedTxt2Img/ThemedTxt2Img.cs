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

        public Button goButton;

        [SerializeField] private List<Prompt> inputPrompts;


        void Update()
        {
            if (MenuManager.Instance.currentMenu == 0)
            {
                goButton.interactable = inputFields.All(field => field.text.Length > 0);
            }
        }

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
            promptResults.ForEach(result =>
            {
                var matchingPrompt = inputPrompts.Find(input => input.Theme == result.theme);
                result.imageGameObject.SetActive(false);
                result.ApplyPromptLabel(matchingPrompt.Text);
            });

            foreach (var diffusionGenerator in diffusionGenerators)
            {
                var matchingPrompt = inputPrompts.Find(input => input.Theme == diffusionGenerator.PromptTheme);
                var matchingPromptResult = promptResults.Find(result => result.theme == diffusionGenerator.PromptTheme);

                yield return PromptHelper.InvokeTxt2ImgGeneration(this, diffusionGenerator, matchingPrompt.Text, matchingPromptResult);

                while (diffusionGenerator.generating)
                {
                    yield return null;
                }
            }
        }
    }
}
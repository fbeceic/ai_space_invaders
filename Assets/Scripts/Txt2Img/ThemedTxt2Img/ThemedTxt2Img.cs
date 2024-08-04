using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class ThemedTxt2Img : MonoBehaviour, IThemedTxt2Img
    {
        public List<StableDiffusionText2Image> diffusionGenerators;

        public List<TMP_InputField> inputFields;

        public List<Prompt> inputPrompts;

        public static ThemedTxt2Img Instance;

        private void Awake()
        {
            Instance = this;
            diffusionGenerators = Resources.FindObjectsOfTypeAll<StableDiffusionText2Image>()
                .Where(obj => obj.isActiveAndEnabled).ToList();
            DontDestroyOnLoad(gameObject);
        }

        public void StartTxt2ImgGeneration()
        {
            inputFields.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.OrdinalIgnoreCase));
            SetInputPrompts();
            RunPrompts();
        }

        public void SetInputPrompts()
        {
            List<Prompt> prompts = new();
            foreach (var inputField in inputFields)
            {
                var fieldTheme = inputField.GetComponent<InputPrompt>().promptTheme;
                prompts.Add(new Prompt { Text = inputField.text, Theme = fieldTheme });
            }

            inputPrompts = prompts;
        }

        public void RunPrompts()
        {
            foreach (var diffusionGenerator in diffusionGenerators)
            {
                var matchingPrompt = inputPrompts.Find(input => input.Theme == diffusionGenerator.PromptTheme);

                diffusionGenerator.PromptTheme = matchingPrompt.Theme;
                diffusionGenerator.Prompt = ExtendPrompt(matchingPrompt.Text, matchingPrompt.Theme, PromptType.Main);
                diffusionGenerator.NegativePrompt = ExtendPrompt("", matchingPrompt.Theme, PromptType.Negative);
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

                if (matchingPrompt.Theme == PromptTheme.Background &&
                    diffusionGenerator.gameObject.GetComponent<Image>() != null)
                {
                    matchingPrompt.Result = diffusionGenerator.gameObject.GetComponent<Image>().sprite;
                }
                else if (diffusionGenerator.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    matchingPrompt.Result = diffusionGenerator.gameObject.GetComponent<SpriteRenderer>().sprite;
                }
            }

            EnableObjectsAndAssignTextures(inputPrompts);
            MenuManager.Instance.ShowMenu(1);
        }

        private static string ExtendPrompt(string prompt, PromptTheme theme, PromptType type)
            => (type == PromptType.Main ? prompt : "") + ", " +
               string.Join(", ", PromptExtensions.Extensions.GetValue(theme).GetValue(type));

        private static void EnableObjectsAndAssignTextures(List<Prompt> inputPrompts)
        {
            var diffusionGenerators = FindObjectsOfType<StableDiffusionText2Image>().ToList();

            foreach (var diffusionGenerator in diffusionGenerators)
            {
                var matchingPrompt = inputPrompts.Find(input => input.Theme == diffusionGenerator.PromptTheme);
                if (diffusionGenerator.gameObject.GetComponent<Image>() != null)
                {
                    diffusionGenerator.gameObject.GetComponent<Image>().sprite = matchingPrompt.Result;
                }

                if (diffusionGenerator.gameObject.GetComponent<Invader>() != null)
                {
                    diffusionGenerator.gameObject.GetComponent<Invader>().ApplyGeneratedSprite(matchingPrompt.Result);
                }
            }
        }
    }
}
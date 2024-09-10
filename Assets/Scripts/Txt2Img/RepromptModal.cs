using System.Collections;
using TMPro;
using Txt2Img.ThemedTxt2Img;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img
{
    public class RepromptModal : MonoBehaviour
    {
        public TextMeshProUGUI themeHeader;

        public Image promptImage;

        public TMP_InputField promptInput;

        public Toggle enhancePromptToggle;

        private PromptResult modalPromptResult;

        private Prompt _prompt;

        void OnEnable()
        {
            modalPromptResult = GetComponent<PromptResult>();
            var editingPromptResult = AIManager.Instance.editingPromptResult;
            _prompt = new()
            {
                Text = editingPromptResult.text,
                Theme = editingPromptResult.theme,
                Result = editingPromptResult.imageGameObject.GetComponent<Image>().sprite
            };
            themeHeader.text = "Reprompt - " + _prompt.Theme.ToThemeString();
            promptImage.sprite = _prompt.Result;
            promptInput.text = _prompt.Text == Constants.DefaultElementPlaceholder ? "" : _prompt.Text;
        
            var placeholder = (TextMeshProUGUI)promptInput.placeholder;
            placeholder.text = "(describe the " + _prompt.Theme.ToThemeString().ToLower() + ")";
        }

        public void RepromptResult()
        {
            modalPromptResult.theme = _prompt.Theme;
            modalPromptResult.text = promptInput.text;
            StartCoroutine(ProcessRepromptingAndApplyResult());
        }

        private IEnumerator ProcessRepromptingAndApplyResult()
        {
            yield return modalPromptResult.ProcessReprompting(enhancePromptToggle.isOn);

            var editingPromptResult = AIManager.Instance.editingPromptResult;
            editingPromptResult.imageGameObject.GetComponent<Image>().sprite = modalPromptResult.imageGameObject.GetComponent<Image>().sprite;
            editingPromptResult.ApplyPromptLabel(promptInput.text);
        }

        public void CloseModal() 
        {
            gameObject.SetActive(false);
            AIManager.Instance.editingPromptResult = null;
        }
    }
}
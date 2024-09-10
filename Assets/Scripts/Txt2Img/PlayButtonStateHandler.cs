using System;
using TMPro;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Txt2Img
{
    public class PlayButtonStateHandler : MonoBehaviour
    {
        public GameObject playText;

        public GameObject generationStateContainer;

        private Button _button;

        public TextMeshProUGUI generatingText;
        
        private AudioSource _audioSource;
        
        void Start()
        {
            _button = GetComponent<Button>();
            _audioSource = GetComponent<AudioSource>();
            _button.onClick.AddListener(PlaySound);
        }

        void Update()
        {
            if (AIManager.Instance.isDiffusionInProgress)
            {
                _button.interactable = false;
                playText.SetActive(false);
                generationStateContainer.SetActive(true);
                generatingText.text = "Generating...";
            }
            else if (AIManager.Instance.editingPromptResult != null)
            {
                
                _button.interactable = false;
                playText.SetActive(false);
                generationStateContainer.SetActive(true);
                generatingText.text = "Switching " + AIManager.Instance.editingPromptResult.theme.ToThemeString();
            }
            else
            {
                _button.interactable = true;
                playText.SetActive(true);
                generationStateContainer.SetActive(false);
            }
        }

        void PlaySound()
        {
            _audioSource.Play();
        }
    }
}
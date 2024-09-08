using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Txt2Img
{
    public class PlayButtonStateHandler: MonoBehaviour
    {
        public GameObject playText;
        
        public GameObject generationStateContainer;
        
        private Button _button;
        
        private AudioSource _audioSource;
        
        void Start()
        {
            _button = GetComponent<Button>();
            _audioSource =  GetComponent<AudioSource>();
            _button.onClick.AddListener(PlaySound);
        }

        void Update()
        {
            if (AIManager.Instance.isDiffusionInProgress)
            {
                _button.interactable = false;
                playText.SetActive(false);
                generationStateContainer.SetActive(true);
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
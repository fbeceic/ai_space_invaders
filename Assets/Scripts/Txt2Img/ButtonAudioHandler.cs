using System;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img
{
    public class ButtonAudioHandler : MonoBehaviour
    {
        private Button _button;

        private AudioSource _audioSource;

        void Start()
        {
            _button = GetComponent<Button>();
            _audioSource = GetComponent<AudioSource>();
            _button.onClick.AddListener(PlaySound);
        }

        void PlaySound()
        {
            if (_audioSource.isActiveAndEnabled)
            {
                _audioSource.Play();
            }
        }
    }
}
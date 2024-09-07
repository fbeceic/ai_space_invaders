using System;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img
{
    public class ButtonStateHandler: MonoBehaviour
    {
        private Button _button;

        void Start()
        {
            _button = GetComponent<Button>();
        }

        void Update() => _button.interactable = !AIManager.Instance.isDiffusionInProgress;
    }
}
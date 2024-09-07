using System;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img
{
    public class ToggleStateHandler: MonoBehaviour
    {
        private Toggle _toggle;

        void Start()
        {
            _toggle = GetComponent<Toggle>();
        }

        void Update() => _toggle.interactable = !AIManager.Instance.isDiffusionInProgress;
    }
}
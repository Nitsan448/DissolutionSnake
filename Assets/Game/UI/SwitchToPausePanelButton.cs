using System;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToPausePanelButton : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanelButtonsParent;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(() => _pausePanelButtonsParent.SetActive(true));
    }
}
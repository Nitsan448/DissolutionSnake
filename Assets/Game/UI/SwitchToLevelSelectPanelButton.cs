using UnityEngine;
using UnityEngine.UI;

public class SwitchToLevelSelectPanelButton : MonoBehaviour
{
    [SerializeField] private GameObject _levelSelectButtonsParent;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(() => _levelSelectButtonsParent.SetActive(true));
    }
}
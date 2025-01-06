using UnityEngine;
using UnityEngine.UI;

public class PanelSwitchButton : MonoBehaviour
{
    [SerializeField] private GameObject _panelToDisable;
    [SerializeField] private GameObject _panelToEnable;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(SwitchPanel);
    }

    private void SwitchPanel()
    {
        _panelToDisable.SetActive(false);
        _panelToEnable.SetActive(true);
    }
}
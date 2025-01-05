using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _pausePanelCanvasGroup;

    public void FadeInPausePanel()
    {
        _pausePanelCanvasGroup.gameObject.SetActive(true);
    }

    public void FadeOutPausePanel()
    {
        _pausePanelCanvasGroup.gameObject.SetActive(false);
    }
}
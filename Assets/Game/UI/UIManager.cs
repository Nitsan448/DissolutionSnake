using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;

    public void FadeInPausePanel()
    {
        _pausePanel.gameObject.SetActive(true);
    }

    public void FadeOutPausePanel()
    {
        _pausePanel.gameObject.SetActive(false);
    }
}
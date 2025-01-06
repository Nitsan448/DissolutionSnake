using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayLevelButton : MonoBehaviour
{
    [SerializeField] private string _levelName = "Level 1";
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(_levelName);
    }
}
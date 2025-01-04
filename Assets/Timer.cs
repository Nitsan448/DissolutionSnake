using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    private int _timeSinceSceneStarted = 0;

    private void Start()
    {
        CountTime().Forget();
    }

    private async UniTask CountTime()
    {
        while (true)
        {
            _text.text = _timeSinceSceneStarted.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _timeSinceSceneStarted += 1;
        }
    }
}
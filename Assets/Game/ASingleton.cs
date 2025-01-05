using System;
using UnityEngine;

public abstract class ASingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Debug.LogWarning($"Multiple instances of singleton of type {typeof(T)} found in scene. Destroying the current one.");
            Destroy(gameObject);
        }

        DoOnAwake();
    }

    protected virtual void DoOnAwake()
    {
    }
}
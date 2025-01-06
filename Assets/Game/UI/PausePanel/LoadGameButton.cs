using UnityEngine;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(DataPersistenceManager.Instance.LoadGame);
    }
}
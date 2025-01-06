using UnityEngine;
using UnityEngine.UI;

public class SaveGameButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(DataPersistenceManager.Instance.SaveGame);
    }
}
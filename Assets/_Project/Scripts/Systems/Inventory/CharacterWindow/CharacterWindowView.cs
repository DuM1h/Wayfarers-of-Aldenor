using UnityEngine;
using UnityEngine.UI;

public class CharacterWindowView : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private Button _equipmentTabButton;
    [SerializeField] private Button _statsTabButton;

    [Header("Panels")]
    [SerializeField] private GameObject _equipmentPanel;
    [SerializeField] private GameObject _statsPanel;

    private void Awake()
    {
        _equipmentTabButton.onClick.AddListener(ShowEquipment);
        _statsTabButton.onClick.AddListener(ShowStats);

        ShowEquipment();
    }

    private void ShowEquipment()
    {
        _equipmentPanel.SetActive(true);
        _statsPanel.SetActive(false);
    }

    private void ShowStats()
    {
        _equipmentPanel.SetActive(false);
        _statsPanel.SetActive(true);
    }
}
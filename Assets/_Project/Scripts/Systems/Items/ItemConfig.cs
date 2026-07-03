using UnityEngine;

public abstract class ItemConfig : ScriptableObject
{
    [Header("Base Item Data")]
    [SerializeField] private string _id;
    [SerializeField] private string _itemName;
    [SerializeField] private string _description;
    [SerializeField] private float _weight;
    [SerializeField] private Sprite _icon;
    public abstract ItemType Type { get; }

    public string Id => _id;
    public string ItemName => _itemName;
    public string Description => _description;
    public float Weight => _weight;
    public Sprite Icon => _icon;
    public bool isStackable => Type != ItemType.Equipment;
}
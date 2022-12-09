using UnityEngine;

[CreateAssetMenu(menuName = "Configurations/Create ItemConfiguration", fileName = "ItemConfiguration", order = 0)]
public class ItemConfiguration : ScriptableObject
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private InventorySlotType inventorySlotType;
    [SerializeField] private PlayerWeaponConfiguration playerWeaponConfiguration;

    public PlayerWeaponConfiguration WeaponConfiguration => playerWeaponConfiguration;

    public InventorySlotType InventorySlotType => inventorySlotType;

    public Sprite ItemSprite => itemSprite;
}
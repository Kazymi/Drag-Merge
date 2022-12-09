using System;
using UnityEngine;

[Serializable]
public class InventorySlotConfiguration
{
    [SerializeField] private InventorySlotType inventorySlotType;
    [SerializeField] private Color color;

    public InventorySlotType InventorySlotType => inventorySlotType;

    public Color Color => color;
}
using System.Collections.Generic;
using EventBusSystem;
using Kuhpik;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventorySlotComponent : MonoBehaviour, IDropHandler
{
    [SerializeField] private Transform itemPos;
    [SerializeField] private Image slotImage;
    [SerializeField] private List<InventorySlotConfiguration> inventorySlotConfigurations;

    private RectTransform rectTransform;
    private GameData gameData;
    private bool isIntitialized;
    private Dictionary<InventorySlotType, Color> slotColors = new Dictionary<InventorySlotType, Color>();
    public ItemComponent CurrentItemComponent { get; private set; }
    public Vector3 SlotPosition => itemPos.position;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        foreach (var inventorySlotConfiguration in inventorySlotConfigurations)
        {
            slotColors.Add(inventorySlotConfiguration.InventorySlotType, inventorySlotConfiguration.Color);
        }
    }

    public void Initialize(GameData gameData)
    {
        this.gameData = gameData;
    }
    
    private void OnDisable()
    {
        isIntitialized = false;
    }

    public void DetachCurrentItem()
    {
        CurrentItemComponent = null;
        slotImage.color = slotColors[InventorySlotType.Default];
    }

    public void Attach(ItemComponent itemComponent)
    {
        itemComponent.DragMovementComponent.Detach();
        CurrentItemComponent = itemComponent;
        itemComponent.DragMovementComponent.InventorySlotComponent = this;
        itemComponent.DragMovementComponent.SetStartPosition(itemPos.position);
        itemComponent.DragMovementComponent.ReturnToStartPosition();
        slotImage.color = slotColors[itemComponent.CurrentItemConfiguration.InventorySlotType];
    }

    public void TryToMerge(ItemComponent itemComponent)
    {
        if (itemComponent.DragMovementComponent.InventorySlotComponent == this)
        {
            return;
        }

        EventBus.RaiseEvent<IMergeItemsSignal>(t => t.TryToMerge(itemComponent, this));
    }

    public void OnDrop(PointerEventData eventData)
    {
        var itemComponent = eventData.pointerDrag.GetComponent<ItemComponent>();
        if (itemComponent == null)
        {
            return;
        }

        if (CurrentItemComponent == null)
        {
            Attach(itemComponent);
        }
        else
        {
            TryToMerge(itemComponent);
        }
    }
}

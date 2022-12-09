using System.Collections.Generic;
using Coffee.UIExtensions;
using EventBusSystem;
using FactoryPool;
using Kuhpik;
using UnityEngine;

public class MergeSystem : GameSystem, IDragItemSignal, IMergeItemsSignal
{
    [SerializeField] private List<MergeConfiguration> mergeConfigurations;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private TemporaryMonoPooled mergeEffect;
    [SerializeField] private UIParticle effectParent;

    private Pool<TemporaryMonoPooled> effectPool;

    public override void OnInit()
    {
        EventBus.Subscribe(this);
        var factory = new FactoryMonoObject<TemporaryMonoPooled>(mergeEffect.gameObject, effectParent.transform);
        effectPool = new Pool<TemporaryMonoPooled>(factory, 1);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(this);
    }

    public void OnItemStartedDrag(ItemComponent itemComponent)
    {
        var itemConfiguration = itemComponent.CurrentItemConfiguration;
        var mergeableConfigurations = new List<ItemConfiguration>();
        foreach (var mergeableConfiguration in mergeConfigurations)
        {
            foreach (var itemKeyConfiguration in mergeableConfiguration.MergeConfigurations)
            {
                if (itemKeyConfiguration == itemConfiguration)
                {
                    var allItem = mergeableConfiguration.MergeConfigurations;
                    var newItemFirst = allItem[0];
                    var newItemSecond = allItem[1];
                    if (newItemFirst == newItemSecond)
                    {
                        if (mergeableConfigurations.Contains(newItemFirst) == false)
                        {
                            mergeableConfigurations.Add(newItemFirst);
                        }
                    }
                    else
                    {
                        var item = newItemFirst == itemConfiguration ? newItemSecond : newItemFirst;
                        if (mergeableConfigurations.Contains(item) == false)
                        {
                            mergeableConfigurations.Add(item);
                        }
                    }
                }
            }
        }

        foreach (var inventorySlot in inventorySystem.InventorySlotComponents)
        {
            if (inventorySlot.CurrentItemComponent == null || inventorySlot.CurrentItemComponent == itemComponent ||
                mergeableConfigurations.Contains(inventorySlot.CurrentItemComponent.CurrentItemConfiguration))
            {
                continue;
            }

            inventorySlot.CurrentItemComponent.SetVisibleLockImage(true);
        }
    }

    public void OnItemEndedDrag(ItemComponent itemComponent)
    {
        var itemConfiguration = itemComponent.CurrentItemConfiguration;
        foreach (var inventorySlotComponent in inventorySystem.InventorySlotComponents)
        {
            if (inventorySlotComponent.CurrentItemComponent != null)
            {
                inventorySlotComponent.CurrentItemComponent.SetVisibleLockImage(false);
            }
        }
    }

    public void TryToMerge(ItemComponent dragItemComponent, InventorySlotComponent slot)
    {
        var slotItem = slot.CurrentItemComponent;
        var result = GetMergingResult(dragItemComponent, slotItem);
        if (result == null)
        {
            return;
        }

        slotItem.DragMovementComponent.Detach();
        slotItem.ReturnToPool();
        dragItemComponent.DragMovementComponent.Detach();
        dragItemComponent.ReturnToPool();
        var newIcon = game.ItemSystem.SpawnItem(result, slot.SlotPosition);
        var effect = effectPool.Pull();
        effect.transform.position = slot.SlotPosition;
        effectParent.RefreshParticles();
        effect.GetComponent<ParticleSystem>().Play();
        slot.Attach(newIcon);
        newIcon.PlaySpawnAnimation();
        EventBus.RaiseEvent<IPlayerMergeItem>(t => t.MergeComplete());
    }

    private ItemConfiguration GetMergingResult(ItemComponent firstItemComponent, ItemComponent secondItemComponent)
    {
        foreach (var mergeConfiguration in mergeConfigurations)
        {
            bool foundFirstKey = false;
            bool foundSecondKey = false;
            foreach (var keyConfigurations in mergeConfiguration.MergeConfigurations)
            {
                if (keyConfigurations == firstItemComponent.CurrentItemConfiguration && foundFirstKey == false)
                {
                    foundFirstKey = true;
                    continue;
                }

                if (keyConfigurations == secondItemComponent.CurrentItemConfiguration)
                {
                    foundSecondKey = true;
                    continue;
                }
            }

            if (foundFirstKey && foundSecondKey)
            {
                return mergeConfiguration.ResultConfiguration;
            }
        }

        return null;
    }
}
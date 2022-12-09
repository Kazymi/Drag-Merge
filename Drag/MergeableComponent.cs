using EventBusSystem;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemComponent), typeof(DragListener))]
public class MergeableComponent : MonoBehaviour
{
    private ItemComponent itemComponent;
    private DragListener dragListener;
    private bool isDrag;
    private void Awake()
    {
        itemComponent = GetComponent<ItemComponent>();
        dragListener = GetComponent<DragListener>();
    }

    private void OnEnable()
    {
        dragListener.beganDrag += OnDragStarted;
        dragListener.endedDown += OnDragEnded;
    }

    private void OnDisable()
    {
        if (isDrag)
        {
            isDrag = false;
            EventBus.RaiseEvent<IDragItemSignal>(t => t.OnItemEndedDrag(itemComponent));
        }
        dragListener.beganDrag -= OnDragStarted;
        dragListener.endedDown -= OnDragEnded;
    }

    private void OnDragStarted(PointerEventData pointerEventData)
    {
        isDrag = true;
        EventBus.RaiseEvent<IDragItemSignal>(t => t.OnItemStartedDrag(itemComponent));
    }

    private void OnDragEnded(PointerEventData pointerEventData)
    {
        isDrag = false;
        EventBus.RaiseEvent<IDragItemSignal>(t => t.OnItemEndedDrag(itemComponent));
    }
}
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DragListener), typeof(RectTransform))]
public class DragMovementComponent : MonoBehaviour
{
    [SerializeField] private float duration;

    private DragListener dragListener;
    private Vector3 startPos;
    private RectTransform rectTransform;
    private Canvas canvas;
    public InventorySlotComponent InventorySlotComponent { get; set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        dragListener = GetComponent<DragListener>();
    }
    
    private void OnEnable()
    {
        dragListener.drag += Move;
        dragListener.endedDown += ReturnToStartDragPosition;
    }

    private void OnDisable()
    {
        dragListener.drag -= Move;
        dragListener.endedDown -= ReturnToStartDragPosition;
    }

    public void Initialize(Canvas canvas)
    {
        this.canvas = canvas;
        transform.rotation = canvas.transform.rotation;
    }
    
    public void SetStartPosition(Vector3 position)
    {
        startPos = position;
    }

    public void TeleportToStartPosition()
    {
        transform.position = startPos;
    }
    public void Detach()
    {
        if (InventorySlotComponent == null)
        {
            return;
        }
        InventorySlotComponent.DetachCurrentItem();
        InventorySlotComponent = null;
    }
    
    private void ReturnToStartDragPosition(PointerEventData pointerEventData)
    {
        transform.DOMove(startPos, duration);
    }

    public void ReturnToStartPosition()
    {
        transform.DOMove(startPos, duration);
    }

    private void Move(PointerEventData pointerEventData)
    {
        Vector3 delta = pointerEventData.delta / canvas.scaleFactor;
        rectTransform.localPosition += delta;
    }
}
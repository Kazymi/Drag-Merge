using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragListener : MonoBehaviour,IPointerDownHandler,IBeginDragHandler,IEndDragHandler,IDragHandler,IDropHandler
{
    [Range(0f,1f)][SerializeField] private float selectedItemAlpha = .6f;
    private CanvasGroup canvasGroup;
    
    public event Action<PointerEventData> pointerDown; 
    public event Action<PointerEventData> beganDrag; 
    public event Action<PointerEventData> endedDown; 
    public event Action<PointerEventData> drag;
    public event Action<PointerEventData> dropped;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown?.Invoke(eventData);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = selectedItemAlpha;
        beganDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1;
        endedDown?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        drag?.Invoke(eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        dropped?.Invoke(eventData);
    }
}
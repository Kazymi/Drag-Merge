using System;
using DG.Tweening;
using FactoryPool;
using Kuhpik;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(DragMovementComponent), typeof(DragListener))]
public class ItemComponent : MonoPooled
{
    [SerializeField] private Image image;
    [SerializeField] private Image lockImage;
    [SerializeField] private GameObject parametersCanvas;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private Vector3 spawnScale;
    [SerializeField] private float duration;
    public ItemConfiguration CurrentItemConfiguration { get; private set; }
    public DragMovementComponent DragMovementComponent { get; private set; }

    private GameData gameData;
    private DragListener dragListener;

    private void Awake()
    {
        dragListener = GetComponent<DragListener>();
        DragMovementComponent = GetComponent<DragMovementComponent>();
    }

    private void OnEnable()
    {
        if (gameData != null && CurrentItemConfiguration && CurrentItemConfiguration.WeaponConfiguration)
        {
            int damage = GetWeaponDamage();
            damageText.text = damage.ToString();
        }

        dragListener.dropped += TryToMerged;
    }

    private void OnDisable()
    {
        dragListener.dropped -= TryToMerged;
    }

    public void SetVisibleLockImage(bool isVisible)
    {
        lockImage.enabled = isVisible;
    }

    public void PlaySpawnAnimation()
    {
        var sequence = DOTween.Sequence();
        var startScale = transform.localScale;
        sequence.Append(transform.DOScale(spawnScale, duration / 2));
        sequence.Append(transform.DOScale(startScale, duration / 2));
    }

    private void TryToMerged(PointerEventData pointerEventData)
    {
        var item = pointerEventData.pointerDrag.GetComponent<ItemComponent>();
        DragMovementComponent.InventorySlotComponent.TryToMerge(item);
    }

    private int GetWeaponDamage()
    {
        return Convert.ToInt32(CurrentItemConfiguration.WeaponConfiguration.Damage +
                                     (CurrentItemConfiguration.WeaponConfiguration.Damage / 100f *
                                      gameData.currentAmountDamage));
    }
    public void Initialize(ItemConfiguration itemConfiguration, Canvas canvas, GameData gameData)
    {
        this.gameData = gameData;
        DragMovementComponent.Initialize(canvas);
        CurrentItemConfiguration = itemConfiguration;
        if (itemConfiguration.WeaponConfiguration != null)
        {
            parametersCanvas.gameObject.SetActive(true);
            levelText.text = itemConfiguration.WeaponConfiguration.Level + " lv.";
            int damage = GetWeaponDamage();
            damageText.text = damage.ToString();
        }
        else
        {
            parametersCanvas.gameObject.SetActive(false);
        }
        
        image.sprite = itemConfiguration.ItemSprite;
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countText;

    public TextMeshProUGUI NameText => nameText;
    public TextMeshProUGUI CountText => countText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public float Height => rectTransform.rect.height;

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.Item.name;
        countText.text = $"X {itemSlot.Count}";
    }
}

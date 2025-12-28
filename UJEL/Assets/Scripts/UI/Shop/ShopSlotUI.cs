using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ShopSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ItemNameText;
    [SerializeField] TextMeshProUGUI ItemCostText;
    [SerializeField] TextMeshProUGUI ItemCountText;

    public ShopSlot shopSlot;
    public ShopSlot thisShopSlot => shopSlot;

    RectTransform rectTransform;
    public float Height => rectTransform.rect.height;

    public void Init(ShopSlot slot)
    {
        shopSlot = slot;

        rectTransform = GetComponent<RectTransform>();

        ItemNameText.text = shopSlot.item.Name;
        string shopCost = shopSlot.cost.ToString();
        ItemCostText.text = $"${shopCost}";

        if (shopSlot.infiniteNumOfItem)
        {
            ItemCountText.text = "inf";
        }
        else
        {
            UpdateCountText(slot);
        }
    }

    public void UpdateCountText(ShopSlot slot)
    {
        string countString = shopSlot.itemCount.ToString();
        if (countString.Length <= 0) Debug.LogError("Count String is length 0.");
        else if (countString.Length == 1) ItemCountText.text = $"x  {countString}";
        else if (countString.Length >= 2) ItemCountText.text = $"x {countString}";
    }

    public void Highlight(bool on)
    {
        if (on) ItemNameText.color = Color.blue;
        else ItemNameText.color = Color.black;
    }
}

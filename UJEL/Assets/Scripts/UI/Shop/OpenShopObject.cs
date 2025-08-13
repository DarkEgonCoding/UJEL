using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShopObject : MonoBehaviour, Interactable
{
    [SerializeField] public List<ShopSlot> shopSlots;

    public void Interact(Transform initiator)
    {
        StartCoroutine(ShopController.instance.OpenShop(shopSlots));
    }
}

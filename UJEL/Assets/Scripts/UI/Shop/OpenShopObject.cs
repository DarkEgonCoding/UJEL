using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShopObject : MonoBehaviour, Interactable
{
    public void Interact(Transform initiator)
    {
        StartCoroutine(ShopController.instance.OpenShop());
    }
}

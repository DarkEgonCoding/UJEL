using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public IEnumerator OpenShop()
    {
        yield return null;
    }
}

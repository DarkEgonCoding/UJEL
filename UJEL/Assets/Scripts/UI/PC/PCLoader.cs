using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCLoader : MonoBehaviour, Interactable
{
    public void Interact(Transform initiator)
    {
        StartCoroutine(PCBox.instance.OpenPCBox());
    }
}

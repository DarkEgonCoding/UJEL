using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePortal : MonoBehaviour, Interactable
{
    [SerializeField] Portal portal;

    public void Interact(Transform initiator)
    {
        if (portal == null)
        {
            Debug.LogError("Interactable portal cannot find portal.");
            return;
        }

        portal.ForceTrigger(PlayerController.Instance);
    }
}

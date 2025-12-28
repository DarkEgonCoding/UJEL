using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(Mathf.Floor(pos.x) + 0.5f, Mathf.Floor(pos.y) + 0.5f, pos.z);
        }
    }

    public void Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            DisablePickup();

            Dialog dialog;
            if (IsVowel(item.Name[0]))
            {
                dialog = new Dialog($"You found an {item.Name}!");
            }
            else
            {
                dialog = new Dialog($"You found a {item.Name}!");
            }
            DialogManager.Instance.ShowDialog(dialog, () => { });
        }
    }

    private void DisablePickup()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void EnablePickup()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private bool IsVowel(char character)
    {
        char lowerChar = char.ToLower(character);
        if (lowerChar == 'a' || lowerChar == 'e' || lowerChar == 'i' || lowerChar == 'o' || lowerChar == 'u')
            return true;

        return false;
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;

        if (Used)
            DisablePickup();
        else
            EnablePickup();
    }
}

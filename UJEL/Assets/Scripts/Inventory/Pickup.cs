using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactable
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;

    public void Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

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

    private bool IsVowel(char character) {
        char lowerChar = char.ToLower(character);
        if (lowerChar == 'a' || lowerChar == 'e' || lowerChar == 'i' || lowerChar == 'o' || lowerChar == 'u')
            return true;

        return false;
    }
}

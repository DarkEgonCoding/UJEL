using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] ItemBase item;

    [SerializeField] Sprite closedSpr;
    [SerializeField] Sprite openSpr;
    SpriteRenderer spriteRenderer;

    public bool Used { get; set; } = false;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedSpr;
        Used = false;
    }

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
            // Check if player is below the chest
            Vector2 chestPos = transform.position;
            Vector2 playerPos = initiator.position;

            if (!(playerPos.y < chestPos.y - 0.5f && Mathf.Abs(playerPos.x - chestPos.x) < 0.5f)) return; // Must be below the chest

            initiator.GetComponent<Inventory>().AddItem(item);
            Used = true;

            spriteRenderer.sprite = openSpr;

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
    
    private void EnableChest()
    {
        GetComponent<SpriteRenderer>().sprite = closedSpr;
    }

    private void DisableChest()
    {
        GetComponent<SpriteRenderer>().sprite = openSpr;
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
            DisableChest();
        else
            EnableChest();
    }
}

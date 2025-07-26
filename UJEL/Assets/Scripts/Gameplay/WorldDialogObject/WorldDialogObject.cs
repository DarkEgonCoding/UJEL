using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldDialogObject : MonoBehaviour, Interactable
{
    [SerializeField] private Dialog dialog;

    [SerializeField] public bool RequireDirection = false;

    public enum RequiredPlayerDirection { Above, Below, Left, Right }
    [SerializeField] public RequiredPlayerDirection requiredDirection;

    public void Interact(Transform initiator)
    {
        if (RequireDirection)
        {
            Vector2 playerDir = initiator.GetComponent<PlayerController>().GetFacingDirection();
            Vector2 direction = (transform.position - initiator.position).normalized;

            switch (requiredDirection)
            {
                case RequiredPlayerDirection.Above:
                    if (playerDir == Vector2.down)
                        DialogManager.Instance.ShowDialog(dialog, () => { });
                    break;
                case RequiredPlayerDirection.Below:
                    if (playerDir == Vector2.up)
                        DialogManager.Instance.ShowDialog(dialog, () => { });
                    break;
                case RequiredPlayerDirection.Left:
                    if (playerDir == Vector2.right)
                        DialogManager.Instance.ShowDialog(dialog, () => { });
                    break;
                case RequiredPlayerDirection.Right:
                    if (playerDir == Vector2.left)
                        DialogManager.Instance.ShowDialog(dialog, () => { });
                    break;
            }
        }
        else
        {
            DialogManager.Instance.ShowDialog(dialog, () => {});
        }
        
    }
}

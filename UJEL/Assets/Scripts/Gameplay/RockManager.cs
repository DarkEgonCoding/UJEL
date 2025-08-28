using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    [SerializeField] LavaManager lavaManager;
    public bool isInLava = false;

    public bool TryPush(Vector2 direction)
    {
        if (lavaManager == null)
            lavaManager = FindObjectOfType<LavaManager>();

        if (isInLava) return false;

        Vector2 targetPos = (Vector2)transform.position + direction;

        Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.solidObjectsLayer | GameLayers.i.interactableLayer);

        if (hit != null)
        {
            return false;
        }

        Vector3Int gridPos = lavaManager.lavaMap.WorldToCell(targetPos);
        if (lavaManager.IsLava(gridPos))
        {
            // Fill lava with ground
            lavaManager.FillLava(gridPos);

            // Rock disappears (falls into lava)
            isInLava = true;
            Destroy(gameObject);

            return true; // player move allowed
        }

        transform.position = targetPos;
        return true;
    }
}

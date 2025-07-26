using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WalkToTileAction : CutsceneAction
{
    [Tooltip("The integer numbers entered into this field will both correct for the player being in the center of the tile by adding 0.5f.")]
    [SerializeField] Vector2Int targetTile;

    public override IEnumerator Play()
    {
        var player = PlayerController.Instance;
        Vector3 targetWorldPos = new Vector3(targetTile.x + 0.5f, targetTile.y + 0.5f);

        // Walk to target tile
        while (Vector3.Distance(player.transform.position, targetWorldPos) > 0.01f)
        {
            Vector3 dir = (targetWorldPos - player.transform.position).normalized;
            Vector2Int intDir = new Vector2Int(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));

            // Only allow cardinal directions
            if (intDir.x != 0) intDir.y = 0;

            yield return player.DoMove(intDir);
        }
    }
}

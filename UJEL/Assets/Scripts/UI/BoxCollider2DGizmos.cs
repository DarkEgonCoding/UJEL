using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D))]
public class BoxCollider2DGizmos : MonoBehaviour
{
    public BoxCollider2D box;

    private void OnDrawGizmos()
    {
        DrawCollider(Color.green);
    }

    private void OnDrawGizmosSelected()
    {
        DrawCollider(Color.cyan);
    }

    private void DrawCollider(Color color)
    {
        if (box == null)
            box = GetComponent<BoxCollider2D>();

        Gizmos.color = color;

        Vector2 offset = (Vector2)transform.position + box.offset;
        Vector2 size = box.size;

        // Apply rotation and scale from the transform
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotationMatrix;

        Gizmos.DrawWireCube(box.offset, size);
    }
}

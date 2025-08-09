using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float CameraX;
    [SerializeField] public float CameraY;
    [SerializeField] public float CameraZ;

    void Update()
    {
        transform.position = player.transform.position + new Vector3(CameraX, CameraY, CameraZ);
    }

    public float pixelsPerUnit = 16f;

    void LateUpdate() {
        Vector3 pos = transform.position;
        pos.x = Mathf.Round(pos.x * pixelsPerUnit) / pixelsPerUnit;
        pos.y = Mathf.Round(pos.y * pixelsPerUnit) / pixelsPerUnit;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}

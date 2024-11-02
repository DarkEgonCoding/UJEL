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
}

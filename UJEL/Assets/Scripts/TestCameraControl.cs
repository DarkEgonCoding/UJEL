using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraControl : MonoBehaviour
{
    public float panSpeed = 8f;
    public Vector2 panLimitX = new Vector2(-50f, 50f);
    public Vector2 panLimitY = new Vector2(-50f, 50f);

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            move.x -= 1;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            move.x += 1;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            move.y += 1;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            move.y -= 1;

        move.Normalize(); // Prevent faster diagonal movement

        transform.position += move * panSpeed * Time.deltaTime;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize -= scroll * 5f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5f, 20f);
    }
}
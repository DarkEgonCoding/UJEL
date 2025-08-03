using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public Image cursor;
    float keyHoldTimer;
    bool isHoldingKey;
    bool isHoldingRL;
    Vector3 holdDirection;
    [SerializeField] float moveDistance = 5f;
    [SerializeField] float initialDelay = 0.4f;
    [SerializeField] float inputRepeatDelay = 0.1f;
    [SerializeField] Vector2 cursorSize = new Vector2(20f, 20f);

    public void HandleUpdate(Action onBack)
    {
        HandleCursorMovement();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(TryFly());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    void HandleCursorMovement()
    {
        Vector3 currentDirection = Vector3.zero;

        // Input tracking
        bool keyDown = Input.GetKey(KeyCode.DownArrow);
        bool keyUp = Input.GetKey(KeyCode.UpArrow);
        bool keyLeft = Input.GetKey(KeyCode.LeftArrow);
        bool keyRight = Input.GetKey(KeyCode.RightArrow);

        // Priority logic: Check for new direction group input and reset state
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            isHoldingKey = true;
            isHoldingRL = false; // stop horizontal hold when switching
            holdDirection = Input.GetKeyDown(KeyCode.DownArrow) ? Vector3.down : Vector3.up;
            keyHoldTimer = Time.time + initialDelay;
            currentDirection = holdDirection;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isHoldingRL = true;
            isHoldingKey = false; // stop vertical hold when switching
            holdDirection = Input.GetKeyDown(KeyCode.RightArrow) ? Vector3.right : Vector3.left;
            keyHoldTimer = Time.time + initialDelay;
            currentDirection = holdDirection;
        }
        else if (isHoldingKey && (keyDown || keyUp))
        {
            if (Time.time >= keyHoldTimer)
            {
                currentDirection = holdDirection;
                keyHoldTimer = Time.time + inputRepeatDelay;
            }
        }
        else if (isHoldingRL && (keyLeft || keyRight))
        {
            if (Time.time >= keyHoldTimer)
            {
                currentDirection = holdDirection;
                keyHoldTimer = Time.time + inputRepeatDelay;
            }
        }

        // Reset holding states if no keys in that direction group are held
        if (!keyDown && !keyUp)
            isHoldingKey = false;
        if (!keyLeft && !keyRight)
            isHoldingRL = false;

        // Move the cursor
        if (currentDirection != Vector3.zero)
        {
            cursor.transform.position += currentDirection * moveDistance;
        }
    }

    IEnumerator TryFly()
    {
        var hit = Physics2D.OverlapBox(cursor.transform.position, cursorSize, 0f);

        if (hit != null)
        {
            var flightPoint = hit.GetComponent<FlightPoint>();
            if (flightPoint != null)
            {
                //yield return StartCoroutine(DialogManager.Instance.ShowDialogText($"Flying to {flightPoint.pointName}!", true));
                Debug.Log($"Flying to {flightPoint.pointName}!");
                //StartCoroutine(FlyToLocation(flightPoint));
            }
        }

        yield return null;
    }

    IEnumerator FlyToLocation(FlightPoint point)
    {
        yield return null;
    }
}

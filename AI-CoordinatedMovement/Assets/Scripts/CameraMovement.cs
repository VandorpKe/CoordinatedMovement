using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Key Input")]
    [SerializeField] KeyCode _keyForwards = KeyCode.W;
    [SerializeField] KeyCode _keyBackwards = KeyCode.S;
    [SerializeField] KeyCode _keyLeft = KeyCode.A;
    [SerializeField] KeyCode _keyRight = KeyCode.D;

    [Header("Camera Settings")]
    [SerializeField] float _cameraSpeed = 25f;

    private void Update()
    {
        Vector3 direction = HandleKeyInput();

        transform.position += direction.normalized * (_cameraSpeed * Time.deltaTime);
    }

    private Vector3 HandleKeyInput()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(_keyForwards))
            direction.z = 1;
        if (Input.GetKey(_keyBackwards))
            direction.z = -1;
        if (Input.GetKey(_keyLeft))
            direction.x = -1;
        if (Input.GetKey(_keyRight))
            direction.x = 1;

        return direction;
    }
}

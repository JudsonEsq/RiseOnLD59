using System;
using UnityEditor.Build;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public InputSystem_Actions controls;

    [SerializeField]
    private float MoveSpeed = 5f;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Vector2 maxDistance = new Vector2(100, 100);

    void Awake()
    {
        controls = new InputSystem_Actions();
    }

    void Update()
    {
        Vector2 dir = controls.Player.Move.ReadValue<Vector2>();
        Vector3 newPosition = cameraTransform.position + new Vector3(dir.x, 0, dir.y) * MoveSpeed * Time.deltaTime;
        if (newPosition.x < maxDistance.x && newPosition.x > -maxDistance.x &&
            newPosition.z < maxDistance.y && newPosition.z > -maxDistance.y)
        { 
            cameraTransform.position = newPosition; 
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

}

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

    void Awake()
    {
        controls = new InputSystem_Actions();
    }

    void Update()
    {
        Vector2 dir = controls.Player.Move.ReadValue<Vector2>();
        cameraTransform.position += new Vector3(dir.x, 0, dir.y) * MoveSpeed * Time.deltaTime;
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

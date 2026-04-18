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
        controls.Player.Move.performed += ctx => Move(ctx);
    }

    void Move(CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();
        cameraTransform.position += new Vector3(direction.x, 0, direction.y) * MoveSpeed;
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

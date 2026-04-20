using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public InputSystem_Actions controls;

    [SerializeField]
    private float MoveSpeed = 5f;

    [SerializeField, Tooltip("Minimum Z bound of the playspace.")]
    private float lowerBound = -12f;

    [SerializeField, Tooltip("How far up the point that makes up the tip of the playspace")]
    private float TopPoint = 60f;

    [SerializeField, Tooltip("How wide the playspace should be - if 65, playspace goes from -65 < x <65")]
    private float WidthBound = 65f;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Vector2 maxDistance = new Vector2(100, 100);

    private float slope = 1f;

    void Awake()
    {
        controls = new InputSystem_Actions();
        slope = (TopPoint - lowerBound) / WidthBound;
    }

    void Update()
    {
        Vector2 dir = controls.Player.Move.ReadValue<Vector2>();
        Vector3 newPosition = cameraTransform.position + new Vector3(dir.x, 0, dir.y) * MoveSpeed * Time.deltaTime;

        float zPos = newPosition.z;
        float xPos = newPosition.x;

        float xMin = (1 / slope) * (zPos - lowerBound) - WidthBound;
        float xMax = -(1 / slope) * (zPos - lowerBound) + WidthBound;
        xPos = Mathf.Clamp(xPos, xMin, xMax);

        Debug.Log("xMin: " + xMin + " XMax: " + xMax + " Slope: " + slope);

        float zMax = TopPoint;

        if(xPos > 0)
        {
            zMax = -slope * (xPos - WidthBound) + lowerBound;
        }
        else
        {
            zMax = slope * (xPos + WidthBound) + lowerBound;
        }

        zPos = Mathf.Clamp(zPos, lowerBound, zMax);
        
        newPosition.x = xPos;
        newPosition.z = zPos;

        cameraTransform.position = newPosition;
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

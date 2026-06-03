using UnityEngine;

public class WheelDragRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

    [Header("Valve Value")]
    [SerializeField] private float valveChangeSpeed = 0.05f;
    [SerializeField, Range(0f, 100f)] private float valveValue = 0f;

    private Camera mainCamera;
    private bool isDragging;
    private Vector2 previousTouchPosition;

    public float ValveValue => valveValue;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            TryStartDragging(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            RotateFromTouch(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    private void TryStartDragging(Vector2 touchPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                isDragging = true;
                previousTouchPosition = touchPosition;
            }
        }
    }

    private void RotateFromTouch(Vector2 currentTouchPosition)
    {
        Vector2 centerScreenPos = mainCamera.WorldToScreenPoint(transform.position);

        Vector2 previousDirection = (previousTouchPosition - centerScreenPos).normalized;
        Vector2 currentDirection = (currentTouchPosition - centerScreenPos).normalized;

        float angle = -Vector2.SignedAngle(previousDirection, currentDirection) * rotationSpeed;

        transform.Rotate(rotationAxis, angle, Space.Self);

        UpdateValveValue(angle);

        previousTouchPosition = currentTouchPosition;
    }

    private void UpdateValveValue(float angle)
    {
        float changeAmount = -angle * valveChangeSpeed;
        valveValue = Mathf.Clamp(valveValue + changeAmount, 0f, 100f);
    }
}
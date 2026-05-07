using UnityEngine;

public class WheelDragRotate : MonoBehaviour
{
    public enum PressureDirection
    {
        Increase,
        Decrease
    }

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

    [Header("Pressure")]
    [SerializeField] private PressureSystem pressureSystem;
    [SerializeField] private PressureDirection pressureDirection = PressureDirection.Increase;
    [SerializeField] private float pressureInputMultiplier = 1f;

    private Camera mainCamera;
    private bool isDragging;
    private Vector2 previousTouchPosition;

    private void Start()
    {
        mainCamera = Camera.main;

        if (pressureSystem == null)
        {
            pressureSystem = GetComponentInParent<PressureSystem>();
        }
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

        SendPressureInput(angle);

        previousTouchPosition = currentTouchPosition;
    }

    private void SendPressureInput(float angle)
    {
        if (pressureSystem == null)
            return;

        float inputAmount = Mathf.Abs(angle) * pressureInputMultiplier;

        if (pressureDirection == PressureDirection.Decrease)
        {
            inputAmount *= -1f;
        }

        pressureSystem.AddPressureInput(inputAmount);
    }
}
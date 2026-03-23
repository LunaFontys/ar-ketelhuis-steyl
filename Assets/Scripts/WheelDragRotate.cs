using UnityEngine;

public class WheelDragRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.3f;
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 1f, 0f);

    [SerializeField] private ParticleSystem steamParticles;
    [SerializeField] private float rotationThreshold = 1080f; // 3 full rotations

    private Camera mainCamera;
    private bool isDragging = false;
    private float totalRotation = 0f;
    private bool steamStopped = false;

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
            Ray ray = mainCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    isDragging = true;
                }
            }
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            float rotationAmount = -touch.deltaPosition.x * rotationSpeed;

            transform.Rotate(rotationAxis, rotationAmount, Space.Self);
            totalRotation += Mathf.Abs(rotationAmount);

            CheckSteamStop();
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    private void CheckSteamStop()
    {
        if (steamStopped)
            return;

        if (totalRotation >= rotationThreshold)
        {
            steamStopped = true;

            if (steamParticles != null)
            {
                steamParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            Debug.Log("Steam stopped!");
        }
    }
}
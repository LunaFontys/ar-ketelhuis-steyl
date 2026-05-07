using UnityEngine;

public class PressureSystem : MonoBehaviour
{
    [Header("Pressure Value")]
    [SerializeField, Range(0f, 1f)] private float pressureValue = 0.5f;

    [Header("Player Input")]
    [SerializeField] private float wheelPressureSpeed = 0.25f;

    [Header("Automatic Disturbance")]
    [SerializeField] private bool useAutomaticDisturbance = true;
    [SerializeField] private float disturbanceInterval = 6f;
    [SerializeField] private float disturbanceDuration = 3f;
    [SerializeField] private float disturbanceStrength = 0.12f;

    [Header("Safe Zone")]
    [SerializeField] private float safeZoneMin = 0.33f;
    [SerializeField] private float safeZoneMax = 0.66f;

    private float disturbanceTimer;
    private float activeDisturbanceTimer;
    private float disturbanceDirection;

    public float PressureValue => pressureValue;
    public bool IsInSafeZone => pressureValue >= safeZoneMin && pressureValue <= safeZoneMax;

    private void Start()
    {
        disturbanceTimer = disturbanceInterval;
    }

    private void Update()
    {
        HandleAutomaticDisturbance();
    }

    public void AddPressureInput(float input)
    {
        pressureValue += input * wheelPressureSpeed * Time.deltaTime;
        pressureValue = Mathf.Clamp01(pressureValue);
    }

    private void HandleAutomaticDisturbance()
    {
        if (!useAutomaticDisturbance)
            return;

        if (activeDisturbanceTimer > 0f)
        {
            pressureValue += disturbanceDirection * disturbanceStrength * Time.deltaTime;
            pressureValue = Mathf.Clamp01(pressureValue);

            activeDisturbanceTimer -= Time.deltaTime;
            return;
        }

        disturbanceTimer -= Time.deltaTime;

        if (disturbanceTimer <= 0f)
        {
            StartDisturbance();
        }
    }

    private void StartDisturbance()
    {
        disturbanceDirection = Random.value > 0.5f ? 1f : -1f;
        activeDisturbanceTimer = disturbanceDuration;
        disturbanceTimer = disturbanceInterval;

        Debug.Log(disturbanceDirection > 0f ? "Pressure disturbance: rising" : "Pressure disturbance: dropping");
    }
}
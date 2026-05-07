using UnityEngine;
using UnityEngine.UI;

public class PressureMeterUI : MonoBehaviour
{
    [Header("Fill")]
    [SerializeField] private Image fillImage;

    [Header("Zone Colors")]
    [SerializeField] private Color lowDangerColor = Color.red;
    [SerializeField] private Color safeColor = Color.green;
    [SerializeField] private Color highDangerColor = Color.red;

    [Header("Thresholds")]
    [SerializeField] private float safeZoneMin = 0.33f;
    [SerializeField] private float safeZoneMax = 0.66f;

    private PressureSystem pressureSystem;

    private void Awake()
    {
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (pressureSystem == null)
        {
            pressureSystem = FindFirstObjectByType<PressureSystem>();
            return;
        }

        if (fillImage == null)
            return;

        float pressure = pressureSystem.PressureValue;

        fillImage.fillAmount = pressure;

        if (pressure < safeZoneMin)
        {
            fillImage.color = lowDangerColor;
        }
        else if (pressure > safeZoneMax)
        {
            fillImage.color = highDangerColor;
        }
        else
        {
            fillImage.color = safeColor;
        }
    }
}
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PressureSyncSender : MonoBehaviour
{
    [Header("Wheel References")]
    [SerializeField] private WheelDragRotate leftValveWheel;
    [SerializeField] private WheelDragRotate rightValveWheel;

    [Header("Server")]
    [SerializeField] private string serverUrl = "http://192.168.1.100:3000/valve-state";
    [SerializeField] private float sendInterval = 0.2f;


    private void Start()
    {
        Debug.Log("PressureSyncSender started on object: " + gameObject.name);
        Debug.Log("PressureSyncSender instance ID: " + GetInstanceID());
        Debug.Log("PressureSyncSender serverUrl = " + serverUrl);
        StartCoroutine(SendValveLoop());
    }

    private static PressureSyncSender activeSender;

    private void Awake()
    {
        if (activeSender != null && activeSender != this)
        {
            Debug.LogWarning("Duplicate PressureSyncSender destroyed: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        activeSender = this;
    }

    private IEnumerator SendValveLoop()
    {
        while (true)
        {
            SendValveValues();
            yield return new WaitForSeconds(sendInterval);
        }
    }

    private void SendValveValues()
    {
        if (leftValveWheel == null || rightValveWheel == null)
        {
            Debug.LogWarning("Valve wheel reference missing.");
            return;
        }

        int leftValue = Mathf.RoundToInt(leftValveWheel.ValveValue);
        int rightValue = Mathf.RoundToInt(rightValveWheel.ValveValue);

        Debug.Log($"Sending valve values - valve1: {leftValue}, valve2: {rightValue}");

        string json = JsonUtility.ToJson(new ValveStateData
        {
            valve1 = leftValue,
            valve2 = rightValue
        });

        StartCoroutine(PostValveState(json));
    }

    private IEnumerator PostValveState(string json)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Failed to send valve values: " + request.error);
            }
            else
            {
                Debug.Log("Valve values sent successfully: " + json);
            }
        }
    }

    [System.Serializable]
    private class ValveStateData
    {
        public int valve1;
        public int valve2;
    }
}
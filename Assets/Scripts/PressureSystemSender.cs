using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PressureSyncSender : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PressureSystem pressureSystem;

    [Header("Server")]
    [SerializeField] private string serverUrl = "http://192.168.1.100:3000/valve-state";
    [SerializeField] private float sendInterval = 0.2f;

    private Coroutine sendRoutine;

    private void Start()
    {
        if (pressureSystem == null)
        {
            pressureSystem = GetComponent<PressureSystem>();
        }

        sendRoutine = StartCoroutine(SendPressureLoop());
    }

    private IEnumerator SendPressureLoop()
    {
        while (true)
        {
            SendPressureValue();
            yield return new WaitForSeconds(sendInterval);
        }
    }

    private void SendPressureValue()
    {
        if (pressureSystem == null)
            return;

        int pressure = Mathf.RoundToInt(pressureSystem.PressureValue * 100f);

        string json = JsonUtility.ToJson(new ValveStateData
        {
            valve1 = pressure,
            valve2 = pressure
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
                Debug.LogWarning("Failed to send pressure: " + request.error);
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
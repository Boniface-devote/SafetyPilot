using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

public class DrivingMistakesTracker : MonoBehaviour
{
    public DrivingMonitor drivingMonitor;
    public Text usernameText;
    public Text overspeeding, sharpTurn, suddenBraking, accident, speedBump, pothole;
    public Button requestGuidanceButton;
    public TMP_Text aiGuidanceText;

    public int overspeedingCount = 0;
    public int sharpTurnCount = 0;
    public int suddenBrakingCount = 0;
    public int accidentCount = 0;
    public int speedBumpCount = 0;
    public int potholeCount = 0;

    private const string API_KEY = "AIzaSyCHb8yYrpc9tmKGe36JBp2ys02zpXeQrFU";
    private const string AI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key=" + API_KEY;

    // Supabase API Info
    private const string SUPABASE_URL = "https://schylceelekdenhwddjc.supabase.co/rest/v1/behavior_performance";
    private const string SUPABASE_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNjaHlsY2VlbGVrZGVuaHdkZGpjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDc0ODI3MDYsImV4cCI6MjA2MzA1ODcwNn0.-jqJqoP5o7ddXC2I3N5LbXct-i0GyM7u0RR5OVqQLAg"; // Replace with your real key

    private string currentUserName = "Guest";

    void Start()
    {
        if (drivingMonitor == null) { Debug.LogError("DrivingMonitor not assigned!"); enabled = false; return; }
        if (requestGuidanceButton != null) requestGuidanceButton.onClick.AddListener(OnRequestGuidanceClicked);
        if (usernameText == null) { Debug.LogError("Username Text not assigned!"); return; }

        currentUserName = PlayerPrefs.GetString("UserName", "Guest");
        usernameText.text = currentUserName;

        if (aiGuidanceText != null) aiGuidanceText.text = "";

        UpdateMistakesDisplay();
    }

    public void TrackMistake(string message)
    {
        switch (message)
        {
            case "Overspeeding!": overspeedingCount++; break;
            case "Sharp Turn!": sharpTurnCount++; break;
            case "Sudden Braking!": suddenBrakingCount++; break;
            case "Accident!": accidentCount++; break;
            case "Slow Down on Speed Bumps!": speedBumpCount++; break;
            case "Watch out! You hit a pothole!": potholeCount++; break;
        }
        UpdateMistakesDisplay();
    }

    void UpdateMistakesDisplay()
    {
        if (overspeeding != null) overspeeding.text = $"Overspeeding: {overspeedingCount}";
        if (sharpTurn != null) sharpTurn.text = $"Sharp Turns: {sharpTurnCount}";
        if (suddenBraking != null) suddenBraking.text = $"Sudden Braking: {suddenBrakingCount}";
        if (accident != null) accident.text = $"Accidents: {accidentCount}";
        if (speedBump != null) speedBump.text = $"Speed Bumps: {speedBumpCount}";
        if (pothole != null) pothole.text = $"Potholes: {potholeCount}";
    }

    void OnRequestGuidanceClicked()
    {
        if (!requestGuidanceButton.interactable) return;
        StartCoroutine(RequestAIGuidance());
        StartCoroutine(PostDrivingMistakesToSupabase());
    }

    IEnumerator RequestAIGuidance()
    {
        requestGuidanceButton.interactable = false;
        float startTime = Time.time;

        string prompt = $"Driving mistakes: {overspeedingCount} overspeeding, {sharpTurnCount} sharp turns, " +
                        $"{suddenBrakingCount} sudden braking, {accidentCount} accidents, " +
                        $"{speedBumpCount} speed bumps, {potholeCount} potholes. " +
                        "Provide a brief and actionable recommendation to improve driving safety in one or two sentences.";

        string jsonPayload = "{\"contents\":[{\"parts\":[{\"text\":\"" + prompt + "\"}]}]}";

        using (UnityWebRequest request = new UnityWebRequest(AI_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            float responseTime = Time.time - startTime;
            Debug.Log($"AI Guidance Response Time: {responseTime} seconds");

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                string aiGuidance = ExtractAIResponse(jsonResponse);
                if (aiGuidanceText != null) aiGuidanceText.text += "\n\n" + aiGuidance;
            }
            else
            {
                Debug.LogError($"API Error: {request.error} (Code: {request.responseCode})");
                if (aiGuidanceText != null) aiGuidanceText.text += "\n\nFailed to get AI guidance. Check your internet connection and try again later";
            }
        }

        requestGuidanceButton.interactable = true;
    }

    IEnumerator PostDrivingMistakesToSupabase()
    {
        var payload = new
        {
            name = currentUserName,
            overspeeding = overspeedingCount,
            sharp_turn = sharpTurnCount,
            sudden_braking = suddenBrakingCount,
            accident = accidentCount,
            speed_bump = speedBumpCount,
            pothole = potholeCount,
            recorded_at = System.DateTime.UtcNow.ToString("o")
        };

        string jsonData = JsonConvert.SerializeObject(payload);

        using (UnityWebRequest request = new UnityWebRequest(SUPABASE_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", SUPABASE_KEY);
            request.SetRequestHeader("Authorization", "Bearer " + SUPABASE_KEY);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Driving mistake data posted successfully to Supabase.");
            }
            else
            {
                Debug.LogError("Failed to post mistake data to Supabase: " + request.error);
            }
        }
    }

    private string ExtractAIResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            JToken candidate = parsedJson["candidates"]?.FirstOrDefault();
            if (candidate == null) return "No candidates in response.";
            JToken part = candidate["content"]?["parts"]?.FirstOrDefault();
            return part?["text"]?.ToString() ?? "No text in response.";
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON parsing error: {ex.Message}");
            return "No valid response from AI.";
        }
    }
}

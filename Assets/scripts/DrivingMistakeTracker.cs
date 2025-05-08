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
    // Reference to the DrivingMonitor script
    public DrivingMonitor drivingMonitor;

    // Display text for each type of mistake
    public Text overspeeding;
    public Text sharpTurn;
    public Text suddenBraking;
    public Text accident;
    public Text speedBump;
    public Text pothole;

    // UI for AI guidance
    public Button requestGuidanceButton;
    public TMP_Text aiGuidanceText;

    // Counters for each type of mistake
    public int overspeedingCount = 0;
    public int sharpTurnCount = 0;
    public int suddenBrakingCount = 0;
    public int accidentCount = 0;
    public int speedBumpCount = 0;
    public int potholeCount = 0;

    private const string API_KEY = "AIzaSyCHb8yYrpc9tmKGe36JBp2ys02zpXeQrFU";
    private const string AI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key=" + API_KEY;

    void Start()
    {
        if (drivingMonitor == null)
        {
            Debug.LogError("DrivingMonitor reference is not assigned in the Inspector!");
            enabled = false;
            return;
        }

        if (requestGuidanceButton == null)
        {
            Debug.LogError("Request Guidance Button not assigned!");
        }
        else
        {
            requestGuidanceButton.onClick.AddListener(OnRequestGuidanceClicked);
        }

        if (aiGuidanceText == null)
        {
            Debug.LogError("AI Guidance Text not assigned!");
        }
        else
        {
            aiGuidanceText.text = "";
        }

        // Initialize the UI display
        UpdateMistakesDisplay();
    }

    // Method to be called by DrivingMonitor whenever ShowAlert is triggered
    public void TrackMistake(string message)
    {
        switch (message)
        {
            case "Overspeeding!":
                overspeedingCount++;
                break;
            case "Sharp Turn!":
                sharpTurnCount++;
                break;
            case "Sudden Braking!":
                suddenBrakingCount++;
                break;
            case "Accident!":
                accidentCount++;
                break;
            case "Slow Down on Speed Bumps!":
                speedBumpCount++;
                break;
            case "Watch out! You hit a pothole!":
                potholeCount++;
                break;
        }

        // Update the UI after each mistake
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
    }

    IEnumerator RequestAIGuidance()
    {
        requestGuidanceButton.interactable = false;
        float startTime = Time.time;

        // Construct the prompt based on mistake counts
        string prompt = $"Driving mistakes: {overspeedingCount} overspeeding, {sharpTurnCount} sharp turns, " +
                        $"{suddenBrakingCount} sudden braking, {accidentCount} accidents, " +
                        $"{speedBumpCount} speed bumps, {potholeCount} potholes. " +
                        "Provide a brief and actionable recommendation to improve driving safety in one or two sentences.";

        // Construct JSON payload
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
                Debug.Log("AI Guidance: " + aiGuidance);

                if (aiGuidanceText != null)
                {
                    aiGuidanceText.text += "\n\n" + aiGuidance; // Append guidance
                }
            }
            else
            {
                Debug.LogError($"API Error: {request.error} (Code: {request.responseCode})");
                if (aiGuidanceText != null)
                {
                    aiGuidanceText.text += "\n\nFailed to get AI guidance.";
                }
            }
        }

        requestGuidanceButton.interactable = true;
    }

    private string ExtractAIResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            JToken candidate = parsedJson["candidates"]?.FirstOrDefault();
            if (candidate == null) return "No candidates in response.";
            JToken part = candidate["content"]?["parts"]?.FirstOrDefault();
            if (part == null) return "No parts in response.";
            return part["text"]?.ToString() ?? "No text in response.";
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON parsing error: {ex.Message}");
            return "No valid response from AI.";
        }
    }
}
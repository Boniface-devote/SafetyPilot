using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;
using Newtonsoft.Json.Linq;
using TMPro;

public class CameraAIButton : MonoBehaviour
{
    public Camera playerCamera;
    public TMP_Text aiResponseText; // UI Text to show AI response
    private Texture2D screenshot;

    private const string API_KEY = "AIzaSyCHb8yYrpc9tmKGe36JBp2ys02zpXeQrFU"; // ⚠️ Move to secure storage in production
    private const string AI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key=" + API_KEY;
    private float captureInterval = 1.0f; // Seconds between captures
    private bool isProcessing = false;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not assigned!");
            enabled = false; // Disable script if camera is missing
            return;
        }

        if (aiResponseText == null)
        {
            Debug.LogError("AI Response Text UI not assigned!");
            enabled = false; // Disable script if text UI is missing
            return;
        }

        aiResponseText.text = "Starting lane change detection...";
        StartCoroutine(ContinuousCaptureAndAnalyze());
    }

    IEnumerator ContinuousCaptureAndAnalyze()
    {
        while (true)
        {
            if (!isProcessing)
            {
                yield return StartCoroutine(CaptureAndSend());
            }
            yield return new WaitForSeconds(captureInterval);
        }
    }

    IEnumerator CaptureAndSend()
    {
        isProcessing = true;
        float startTime = Time.time;

        yield return StartCoroutine(CaptureFrame());
        if (screenshot != null)
        {
            yield return StartCoroutine(SendToAI());
        }
        else
        {
            Debug.LogError("Failed to capture screenshot.");
            if (aiResponseText != null)
                aiResponseText.text = "Failed to capture screenshot.";
        }

        isProcessing = false;
        float responseTime = Time.time - startTime;
        Debug.Log($"Frame Analysis Time: {responseTime} seconds");
    }

    IEnumerator CaptureFrame()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        playerCamera.targetTexture = renderTexture;
        playerCamera.Render();

        RenderTexture.active = renderTexture;
        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        playerCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
    }

    IEnumerator SendToAI()
    {
        if (screenshot == null)
        {
            Debug.LogError("No screenshot to send to AI.");
            yield break;
        }

        byte[] imageBytes = screenshot.EncodeToJPG(75); // Reduce quality for smaller payload
        string imageBase64 = System.Convert.ToBase64String(imageBytes);
        string jsonPayload = "{\"contents\":[{\"parts\":[{\"text\":\"Detect if the player's white car is in the correct driving side of ugandan road,answer correct lanes or wrong lanes (must drive on the either of the 2 left lanes and not on the right ones like in uganda).\"},{\"inline_data\":{\"mime_type\":\"image/jpeg\",\"data\":\"" + imageBase64 + "\"}}]}]}";

        using (UnityWebRequest request = new UnityWebRequest(AI_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                string aiText = ExtractAIResponse(jsonResponse);
                Debug.Log("AI Response: " + aiText);

                if (aiResponseText != null)
                {
                    aiResponseText.text = aiText; // Overwrite to show latest response
                }
            }
            else
            {
                Debug.LogError($"API Error: {request.error}");
                if (aiResponseText != null)
                    aiResponseText.text = "Failed to get AI response. Check your internet connection or API key.";
            }
        }
    }

    private string ExtractAIResponse(string jsonResponse)
    {
        try
        {
            JObject parsedJson = JObject.Parse(jsonResponse);
            return parsedJson["candidates"][0]["content"]["parts"][0]["text"].ToString();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse AI response: {ex.Message}");
            return "No valid response from AI.";
        }
    }

    void OnDestroy()
    {
        if (screenshot != null)
        {
            Destroy(screenshot); // Clean up screenshot texture
        }
    }
}
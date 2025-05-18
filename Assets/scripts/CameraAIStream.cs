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
    public Button analyzeButton;
    public TMP_Text aiResponseText; // ✅ UI Text to show AI response
    private Texture2D screenshot;

    private const string API_KEY = "AIzaSyCHb8yYrpc9tmKGe36JBp2ys02zpXeQrFU";
    private const string AI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key=" + API_KEY;

    private float startTime;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not assigned!");
        }

        if (analyzeButton != null)
        {
            analyzeButton.onClick.AddListener(OnAnalyzeButtonClicked);
        }
        else
        {
            Debug.LogError("Analyze button not assigned!");
        }

        if (aiResponseText != null)
        {
            aiResponseText.text = "";
        }
        else
        {
            Debug.LogError("AI Response Text UI not assigned!");
        }
    }

    void OnAnalyzeButtonClicked()
    {
        StartCoroutine(CaptureAndSend());
    }

    IEnumerator CaptureAndSend()
    {
        analyzeButton.GetComponent<Image>().color = Color.green;
        startTime = Time.time;

        yield return StartCoroutine(CaptureFrame());
        yield return StartCoroutine(SendToAI());

        analyzeButton.GetComponent<Image>().color = Color.white;
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
        byte[] imageBytes = screenshot.EncodeToJPG();
        string imageBase64 = System.Convert.ToBase64String(imageBytes);
        string jsonPayload = "{\"contents\":[{\"parts\":[{\"text\":\"Analyze this driving scene and suggest a simple best course of action.\"},{\"inline_data\":{\"mime_type\":\"image/jpeg\",\"data\":\"" + imageBase64 + "\"}}]}]}";

        using (UnityWebRequest request = new UnityWebRequest(AI_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            float responseTime = Time.time - startTime;
            Debug.Log($"AI Response Time: {responseTime} seconds");

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                string aiText = ExtractAIResponse(jsonResponse);
                Debug.Log("AI Response: " + aiText);

                if (aiResponseText != null)
                {
                    aiResponseText.text += "\n\n" + aiText; // ✅ Append instead of overwrite
                }
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                if (aiResponseText != null)
                    aiResponseText.text = "Failed to get AI response. Check your internet connection and try again later";
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
        catch
        {
            return "No valid response from AI.";
        }
    }
}

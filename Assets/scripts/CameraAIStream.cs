using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json.Linq;

public class CameraAIStream : MonoBehaviour
{
    public Camera playerCamera;
    private Texture2D screenshot;
    private bool isRunning = true;
    private const string API_KEY = "AIzaSyCHb8yYrpc9tmKGe36JBp2ys02zpXeQrFU";
    private const string AI_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key=" + API_KEY;

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not assigned!");
            return;
        }
        StartCoroutine(CaptureAndSendRoutine());
    }

    IEnumerator CaptureAndSendRoutine()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(3f);
            yield return StartCoroutine(CaptureFrame());
            yield return StartCoroutine(SendToAI());
        }
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

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                string aiText = ExtractAIResponse(jsonResponse);
                Debug.Log("AI Response: " + aiText);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
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

    public void StopAnalysis()
    {
        isRunning = false;
        Debug.Log("Analysis Stopped.");
    }
}

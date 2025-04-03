using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;  // For Button reference
using System.Collections.Generic;
using Newtonsoft.Json.Linq;  // Import JSON parsing library

public class AIRequest : MonoBehaviour
{
    public CameraCapture cameraCapture;
    public Button assistButton;  // Reference to the button
    private string apiKey = "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"; // Replace with your API key
    private string apiUrl = "https://openrouter.ai/api/v1/chat/completions";

    private float startTime;  // For measuring response time

    public void SendImageToAI()
    {
        StartCoroutine(UploadImage());
    }

    IEnumerator UploadImage()
    {
        // Change the button color to light green when pressed
        assistButton.GetComponent<Image>().color = Color.green;

        // Start the timer when the request is sent
        startTime = Time.time;

        byte[] imageData = cameraCapture.CaptureFrameAsPNG();
        string base64Image = System.Convert.ToBase64String(imageData);

        string jsonPayload = "{\"model\": \"google/learnlm-1.5-pro-experimental:free\", \"messages\": [{ \"role\": \"user\", \"content\": [{ \"type\": \"text\", \"text\": \"Provide a simple and short Guide on driving based on this image.\" }, { \"type\": \"image_url\", \"image_url\": { \"url\": \"data:image/png;base64," + base64Image + "\" } } ] } ]}";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // Stop the timer when the response is received
                float responseTime = Time.time - startTime;
                Debug.Log($"AI Response Time: {responseTime} seconds");

                string jsonResponse = request.downloadHandler.text;
                string guidanceText = ParseAIResponse(jsonResponse);
                Debug.Log("AI Guidance: " + guidanceText);
            }

            // Reset button color after response
            assistButton.GetComponent<Image>().color = Color.white;  // Reset to original color
        }
    }

    string ParseAIResponse(string jsonResponse)
    {
        try
        {
            JObject parsedResponse = JObject.Parse(jsonResponse);
            JArray choices = (JArray)parsedResponse["choices"];
            if (choices != null && choices.Count > 0)
            {
                string responseText = (string)choices[0]["message"]["content"];
                return responseText;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing AI response: " + e.Message);
        }
        return "No guidance received.";
    }
}

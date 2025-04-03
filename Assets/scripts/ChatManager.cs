using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField userInputField; // User input field for chat
    public Button sendButton; // Send button to trigger message
    public TMP_Text chatDisplay; // Text field to display the conversation
    public GameObject chatPanel; // Chat Panel UI

    private string apiUrl = "https://api.groq.com/openai/v1/chat/completions";
    private string apiKey = "Bearer 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"; // Your API key

    void Start()
    {
        sendButton.onClick.AddListener(SendMessageToAPI); // Bind send button click event
    }

    void SendMessageToAPI()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            DisplayMessage("User", userMessage); // Display user message in chat
            userInputField.text = ""; // Clear input field
            StartCoroutine(FetchAIResponse(userMessage)); // Send request to Groq API
        }
    }

    // Display message in the chat
    void DisplayMessage(string sender, string message)
    {
        chatDisplay.text += $"\n{sender}: {message}";
        chatDisplay.pageToDisplay = chatDisplay.text.Length / 100; // Auto scroll
    }

    // Coroutine to handle the HTTP request
    IEnumerator FetchAIResponse(string userMessage)
    {
        // Prepare JSON payload
        string jsonPayload = "{\"model\": \"llama-3.3-70b-versatile\", \"messages\": [{\"role\": \"user\", \"content\": \"" + userMessage + "\"}]}";

        // Create UnityWebRequest with POST method
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] jsonData = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(jsonData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", apiKey);

        // Wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response JSON
            string response = request.downloadHandler.text;
            string aiMessage = ParseAIResponse(response);
            DisplayMessage("AI", aiMessage); // Display AI response
        }
        else
        {
            // Handle any errors
            DisplayMessage("Error", "Failed to connect to AI service.");
            Debug.LogError(request.error);
        }
    }

    // Parse the AI response from Groq API
    string ParseAIResponse(string response)
    {
        // Find the response in JSON (you may need to adjust based on actual API response format)
        int startIndex = response.IndexOf("\"content\":") + 11;
        int endIndex = response.IndexOf("\"", startIndex);
        return response.Substring(startIndex, endIndex - startIndex);
    }
}

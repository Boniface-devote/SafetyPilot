using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SupabaseManager : MonoBehaviour
{
    private const string supabaseUrl = "https://schylceelekdenhwddjc.supabase.co/rest/v1/users";
    private const string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNjaHlsY2VlbGVrZGVuaHdkZGpjIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDc0ODI3MDYsImV4cCI6MjA2MzA1ODcwNn0.-jqJqoP5o7ddXC2I3N5LbXct-i0GyM7u0RR5OVqQLAg";

    [System.Serializable]
    public class User
    {
        public string name;
        public string created_at;

        public User(string name)
        {
            this.name = name;
            this.created_at = System.DateTime.UtcNow.ToString("o"); // ISO 8601 format
        }
    }

    // Public method to register a user with duplicate check
    public void RegisterUserOnline(string userName)
    {
        StartCoroutine(CheckAndRegisterUser(userName));
    }

    // Step 1: Check if the user already exists before inserting
    private IEnumerator CheckAndRegisterUser(string userName)
    {
        string queryUrl = $"{supabaseUrl}?name=eq.{UnityWebRequest.EscapeURL(userName)}&select=*";

        UnityWebRequest checkRequest = UnityWebRequest.Get(queryUrl);
        checkRequest.SetRequestHeader("apikey", apiKey);
        checkRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return checkRequest.SendWebRequest();

        if (checkRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error checking user: " + checkRequest.error);
            yield break;
        }

        string jsonResponse = checkRequest.downloadHandler.text;

        if (jsonResponse != "[]")
        {
            Debug.LogWarning($"User '{userName}' already exists.");
            // Optionally: notify UI here
            yield break;
        }

        // If user does not exist, insert
        StartCoroutine(PostUserToSupabase(userName));
    }

    // Step 2: Insert user if no duplicate
    private IEnumerator PostUserToSupabase(string userName)
    {
        User newUser = new User(userName);
        string jsonData = JsonUtility.ToJson(newUser);

        UnityWebRequest request = new UnityWebRequest(supabaseUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("apikey", apiKey);
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Successfully posted user to Supabase!");
        }
        else
        {
            Debug.LogError("Failed to post user: " + request.error);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class DisplayUsername : MonoBehaviour
{
    public Text usernameText;

    void Start()
    {
        if (usernameText == null)
        {
            Debug.LogError("Username Text is not assigned in the Inspector!");
            return;
        }

        // Retrieve the stored username from PlayerPrefs
        string userName = PlayerPrefs.GetString("UserName", "Guest");

        // Display the username on the UI Text element
        usernameText.text = "Welcome, " + userName + "!";
    }
}
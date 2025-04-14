using UnityEngine;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    public GameObject chatPanel;      // Assign ChatPanel in Inspector
    public Button closeButton;        // Assign CloseButton in Inspector
    public Button openChatButton;     // Assign Chat Panel UI Button in Inspector

    private bool isPaused = false;

    void Start()
    {
        chatPanel.SetActive(false); // Hide chat panel initially
        closeButton.onClick.AddListener(CloseChatPanel); // Assign close function
        openChatButton.onClick.AddListener(OpenChatPanel); // Assign open function
    }

    void Update()
    {
        // Listen for the P key to open chat and pause
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Only open if not already open
            if (!chatPanel.activeSelf)
            {
                OpenChatPanel();
            }
        }
    }

    public void OpenChatPanel()
    {
        Time.timeScale = 0f; // Pause the game
        isPaused = true;
        chatPanel.SetActive(true); // Show chat panel
    }

    public void CloseChatPanel()
    {
        chatPanel.SetActive(false); // Hide chat panel
        if (isPaused)
        {
            Time.timeScale = 1f; // Resume the game
            isPaused = false;
        }
    }
}

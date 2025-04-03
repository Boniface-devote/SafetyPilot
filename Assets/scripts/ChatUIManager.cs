using UnityEngine;
using UnityEngine.UI;

public class ChatUIManager : MonoBehaviour
{
    public GameObject chatPanel; // Assign ChatPanel in Inspector
    public Button closeButton; // Assign CloseButton in Inspector

    void Start()
    {
        chatPanel.SetActive(false); // Hide chat panel initially
        closeButton.onClick.AddListener(CloseChatPanel); // Assign close function
    }

    public void ToggleChatPanel()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
    }

    public void CloseChatPanel()
    {
        chatPanel.SetActive(false);
    }
}

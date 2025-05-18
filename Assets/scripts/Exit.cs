using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    public string sampleSceneName = "SampleScene"; // Ensure this matches your scene name
    public Button exitButton; // Assign the UI Button in the Inspector
    public Button closeButton; // New: Close button for mobile UI
    public GameObject resultsPanel; // Assign the results panel GameObject in the Inspector
    public GameObject uiPanel; // Assign the main UI panel

    void Start()
    {
        // Attach button click event if a button is assigned
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(LoadSampleScene);
        }

        // Attach the close button to the same behavior as Escape key
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnClosePressed);
        }

        // Ensure results panel is initially inactive
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClosePressed();
        }
    }

    public void OnClosePressed()
    {
        if (resultsPanel != null)
        {
            StartCoroutine(ShowResultsAndLoadScene());
        }
        else
        {
            Debug.LogWarning("Results panel not assigned! Loading scene directly.");
            LoadSampleScene();
        }
    }

    private System.Collections.IEnumerator ShowResultsAndLoadScene()
    {
        // Activate the results panel
        resultsPanel.SetActive(true);
        if (uiPanel != null) uiPanel.SetActive(false);

        // Wait for 30 seconds
        yield return new WaitForSeconds(30f);

        // Load the scene
        LoadSampleScene();
    }

    public void LoadSampleScene()
    {
        // Load the scene
        SceneManager.LoadScene(sampleSceneName);
        Debug.Log("Switching to Sample Scene...");
    }
}

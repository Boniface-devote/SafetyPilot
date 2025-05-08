using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    public string sampleSceneName = "SampleScene"; // Ensure this matches your scene name
    public Button exitButton; // Assign the UI Button in the Inspector
    public GameObject resultsPanel; // Assign the results panel GameObject in the Inspector
    public GameObject uiPanel;
    void Start()
    {
        // Attach button click event if a button is assigned
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(LoadSampleScene);
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
    }

    private System.Collections.IEnumerator ShowResultsAndLoadScene()
    {
        // Activate the results panel
        resultsPanel.SetActive(true);
        uiPanel.SetActive(false);

        // Wait for 30 seconds
        yield return new WaitForSeconds(30f);

        // Load the scene
        LoadSampleScene();
    }

    public void LoadSampleScene()
    {
        // Ensure the scene exists before loading
        if (SceneManager.GetSceneByName(sampleSceneName) != null)
        {
            SceneManager.LoadScene(sampleSceneName);
            Debug.Log("Switching to Sample Scene...");
        }
        else
        {
            Debug.LogError("Scene not found! Make sure it's added in Build Settings.");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{
    public string sampleSceneName = "SampleScene"; // Ensure this matches your scene name
    public Button exitButton; // Assign the UI Button in the Inspector

    void Start()
    {
        // Attach button click event if a button is assigned
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(LoadSampleScene);
        }
    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadSampleScene();
        }
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

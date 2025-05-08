using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegistrationManager : MonoBehaviour
{
    public InputField nameInputField;
    public GameObject menuObject;
    public Text message;
    void Start()
    {
        if (nameInputField == null)
        {
            Debug.LogError("Name Input Field is not assigned in the Inspector!");
        }
        if (menuObject == null)
        {
            Debug.LogError("Menu Object is not assigned in the Inspector!");
        }

        // Check if user has already registered
        if (PlayerPrefs.HasKey("UserName"))
        {
            // If registered, directly activate the menu
            if (menuObject != null)
            {
                menuObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Menu Object is not assigned!");
            }
            // Optionally load a new scene
            // SceneManager.LoadScene("MenuScene");
        }
    }

    public void Register()
    {
        string userName = nameInputField.text.Trim();
        if (!string.IsNullOrEmpty(userName))
        {
            // Store the name and mark registration as complete
            PlayerPrefs.SetString("UserName", userName);
            PlayerPrefs.SetInt("IsRegistered", 1);
            PlayerPrefs.Save();

            // Activate the menu object
            if (menuObject != null)
            {
                menuObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Menu Object is not assigned!");
            }

            // Optionally load a new scene
            // SceneManager.LoadScene("MenuScene");
        }
        else
        {
            message.text = "Please enter a valid name!";
            Debug.LogWarning("Please enter a valid name!");
        }
    }
}
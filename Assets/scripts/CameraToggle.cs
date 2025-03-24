using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    public Camera mainCamera;   // Assign the main camera in the inspector
    public Camera frontCamera;  // Assign the front camera in the inspector

    private Camera activeCamera;

    void Start()
    {
        // Ensure both cameras are assigned
        if (mainCamera == null || frontCamera == null)
        {
            Debug.LogError("Please assign both cameras in the inspector!");
            return;
        }

        // Set the main camera as active by default
        activeCamera = mainCamera;
        mainCamera.enabled = true;
        frontCamera.enabled = false;
    }

    void Update()
    {
        // Listen for the "C" key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCamera();
        }
    }

    void ToggleCamera()
    {
        // Switch between cameras
        if (activeCamera == mainCamera)
        {
            mainCamera.enabled = false;
            frontCamera.enabled = true;
            activeCamera = frontCamera;
        }
        else
        {
            frontCamera.enabled = false;
            mainCamera.enabled = true;
            activeCamera = mainCamera;
        }

        Debug.Log("Switched to: " + activeCamera.name);
    }
}

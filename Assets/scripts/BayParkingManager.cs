using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ParkingManager : MonoBehaviour
{
    public Transform playerCar;
    public Transform bayParkingZone;
    public Transform parallelParkingZone;
    public float parkingThreshold = 3f;

    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timerText; // Optional: to show remaining time to the player

    private bool bayParked = false;
    private bool parallelParked = false;
    private float resultDisplayTime = 2f;
    private float resultTimer = 0f;

    private bool stageComplete = false;
    private bool hasFailed = false;

    private float stageTimeLimit = 120f; // Time limit for the stage in seconds (e.g., 2 minutes)
    private float remainingTime; // Track remaining time during the stage

    public DrivingMonitor drivingMonitor; // Reference to DrivingMonitor script

    void Start()
    {
        instructionText.text = "Park in the Bay first!";
        resultText.text = "";
        remainingTime = stageTimeLimit; // Set the remaining time to the time limit

        if (drivingMonitor == null)
        {
            drivingMonitor = Object.FindFirstObjectByType<DrivingMonitor>();
        }
    }

    void Update()
    {
        if (hasFailed || stageComplete) return;

        // Countdown the remaining time
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            hasFailed = true;
            ShowResult("Time's up! Press r or Restart button to restart.");
            instructionText.text = "";

            if (drivingMonitor != null)
            {
                drivingMonitor.ShowAlert("Parking Failed! Time's up.", 10);
            }
        }

        // Update the timer UI (optional)
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.Max(0, Mathf.FloorToInt(remainingTime)) + "s";
        }

        if (!bayParked)
        {
            if (IsCarInParkingZone(bayParkingZone))
            {
                bayParked = true;
                ShowResult("Well done! You parked in the bay!");
                instructionText.text = "Next: Go to the Parallel Parking Zone!";
                if (drivingMonitor != null)
                {
                    drivingMonitor.ShowAlert("Bay Parking Complete!");
                    drivingMonitor.playerScore += 10;
                    drivingMonitor.UpdateScoreDisplay();
                }
            }
        }
        else if (!parallelParked)
        {
            if (IsCarInParkingZone(parallelParkingZone))
            {
                parallelParked = true;
                stageComplete = true;
                CheckStageCompletion();
            }
        }

        // Simulate failure condition
        if (playerCar.position.y < -1f && !hasFailed)
        {
            hasFailed = true;
            ShowResult("You hit an obstacle. Press R to restart.");
            instructionText.text = "";

            if (drivingMonitor != null)
            {
                drivingMonitor.ShowAlert("Parking Failed!", 10);
            }
        }

        // Restart with lowercase 'r'
        if (Input.anyKeyDown && Input.inputString == "r")
        {
            SceneManager.LoadScene("Parking");
        }

        // Hide result text after timer
        if (resultTimer > 0)
        {
            resultTimer -= Time.deltaTime;
            if (resultTimer <= 0)
            {
                resultText.text = "";
            }
        }
    }

    // Function to check if the car is fully inside the parking zone
    bool IsCarInParkingZone(Transform parkingZone)
    {
        Collider zoneCollider = parkingZone.GetComponent<Collider>();
        if (zoneCollider != null)
        {
            // Check if the car is within the bounds of the parking zone (using the collider bounds)
            Bounds zoneBounds = zoneCollider.bounds;
            Bounds carBounds = playerCar.GetComponent<Collider>().bounds;

            // Check if the car's bounds are entirely inside the parking zone's bounds
            return zoneBounds.Contains(carBounds.min) && zoneBounds.Contains(carBounds.max);
        }

        return false;
    }

    void ShowResult(string message)
    {
        resultText.text = message;
        resultTimer = resultDisplayTime;
    }

    void CheckStageCompletion()
    {
        // Check if the stage was completed within the time limit and with the required score
        if (drivingMonitor != null)
        {
            if (remainingTime > 0 && drivingMonitor.playerScore >= 110)
            {
                resultText.text = "Stage Completed!";
                drivingMonitor.ShowAlert("Stage Completed!");
                // Start the coroutine to delay the scene loading
                StartCoroutine(LoadNextSceneAfterDelay(3f)); // 3 seconds delay
            }
            else
            {
                resultText.text = "Repeat Stage";
                drivingMonitor.ShowAlert("Repeat Stage. Try Again!");
            }
        }
    }
    // Coroutine to handle the delay and scene loading
    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        SceneManager.LoadScene("SampleScene");   // Load the next scene
    }
}

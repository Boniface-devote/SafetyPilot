using UnityEngine;
using TMPro;

public class DrivingMonitor : MonoBehaviour
{
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    public Rigidbody carRigidbody;
    public TextMeshProUGUI alertText;
    public Transform alertCanvas;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI scoreText;

    public float speedLimit = 50f;
    public float sharpTurnThreshold = 35f;
    public float alertDuration = 2f;
    public int playerScore = 100;

    private float previousSpeed;
    private float alertTimer = 0f;
    private float safeDrivingTimer = 0f;
    private Camera mainCamera;
    private bool isDrivingSafely = true;

    void Start()
    {
        mainCamera = Camera.main;
        if (speedText != null) speedText.text = "Speed: 0 km/h";
        UpdateScoreDisplay();

        // ... (Your existing Start() code) ...

        // Check if a GameObject with the "SpeedBump" tag exists.
        GameObject speedBump = GameObject.FindGameObjectWithTag("SpeedBump");
        GameObject wheelColliderObject = GameObject.FindGameObjectWithTag("WheelCollider");

      
    }

    void Update()
    {
        MonitorDriving();
        HandleAlertDisplay();
        if (alertCanvas) FaceCamera();
        UpdateSpeedDisplay();
        RewardSafeDriving();
    }

    void MonitorDriving()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
        float steeringAngle = Mathf.Max(Mathf.Abs(frontLeftWheel.steerAngle), Mathf.Abs(frontRightWheel.steerAngle));

        isDrivingSafely = true;

        if (steeringAngle > sharpTurnThreshold)
        {
            ShowAlert("Sharp Turn!", 2);
            isDrivingSafely = false;
        }

        if (currentSpeed > speedLimit)
        {
            ShowAlert("Overspeeding!", 10);
            isDrivingSafely = false;
        }

        previousSpeed = currentSpeed;
    }

    void ShowAlert(string message, int penalty)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.gameObject.SetActive(true);
            alertTimer = alertDuration;
            ReduceScore(penalty);
        }
    }

    void HandleAlertDisplay()
    {
        if (alertTimer > 0)
        {
            alertTimer -= Time.deltaTime;
            if (alertTimer <= 0) alertText.gameObject.SetActive(false);
        }
    }

    void FaceCamera()
    {
        if (mainCamera)
        {
            alertCanvas.LookAt(mainCamera.transform);
            alertCanvas.Rotate(0, 180, 0);
        }
    }

    void UpdateSpeedDisplay()
    {
        if (speedText != null)
        {
            float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
            speedText.text = "Speed: " + currentSpeed.ToString("F1") + " km/h";
        }
    }

    void ReduceScore(int penalty)
    {
        playerScore -= penalty;
        if (playerScore < 0) playerScore = 0;
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + playerScore;
        }
    }

    void RewardSafeDriving()
    {
        if (isDrivingSafely)
        {
            safeDrivingTimer += Time.deltaTime;
            if (safeDrivingTimer >= 5f)
            {
                playerScore += 5;
                safeDrivingTimer = 0f;
                UpdateScoreDisplay();
            }
        }
        else
        {
            safeDrivingTimer = 0f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;

        if (collision.gameObject.CompareTag("SpeedBump"))
        {
            if (currentSpeed > 25f) // Only show alert if speed is above 20 km/h
            {
                ShowAlert("SpeedBump Collision!", 5);
                isDrivingSafely = false;
            }

        }
        else
        {
            ShowAlert("Accident!", 10); // Any other collision = accident
            isDrivingSafely = false;
        }
    }
}
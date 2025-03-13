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
    public Transform alertCanvas; // Assign World Space Canvas above the car
    public TextMeshProUGUI speedText; // Reference to TextMeshProUGUI for speed display

    public float speedLimit = 50f;
    public float harshBrakeThreshold = 10f;
    public float sharpTurnThreshold = 35f;
    public float handBrakeTorque = 3000f;
    public float harshBrakeTiltThreshold = 1.07f;
    public float alertDuration = 2f;

    private float previousSpeed;
    private float alertTimer = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (speedText != null) speedText.text = "Speed: 0 km/h"; // Set initial speed
    }

    void Update()
    {
        MonitorDriving();
        HandleAlertDisplay();
        if (alertCanvas) FaceCamera();
        UpdateSpeedDisplay(); // Update the speed display in each frame
    }

    void MonitorDriving()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f; // Convert to km/h
        float accelerationInput = Input.GetAxis("Vertical");
        float steeringAngle = Mathf.Max(Mathf.Abs(frontLeftWheel.steerAngle), Mathf.Abs(frontRightWheel.steerAngle));
        float carTiltX = transform.rotation.eulerAngles.x;
        float forwardVelocity = Vector3.Dot(carRigidbody.linearVelocity, transform.forward);

        if (carTiltX > 180f) carTiltX -= 360f;

        // Detect reversing
        if (accelerationInput < 0 && forwardVelocity < -0.5f) ShowAlert("Reversing!");

        // Detect harsh braking
        float speedDifference = previousSpeed - currentSpeed;
        if (speedDifference > harshBrakeThreshold || Mathf.Abs(carTiltX) >= harshBrakeTiltThreshold)
        {
            ShowAlert("Harsh Braking!");
            ApplyHandBrake();
        }

        // Detect sharp turns
        if (steeringAngle > sharpTurnThreshold) ShowAlert("Sharp Turn!");

        // Detect overspeeding
        if (currentSpeed > speedLimit) ShowAlert("Overspeeding!");

        previousSpeed = currentSpeed;
    }

    void ApplyHandBrake()
    {
        rearLeftWheel.brakeTorque = handBrakeTorque;
        rearRightWheel.brakeTorque = handBrakeTorque;
    }

    void ShowAlert(string message)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.gameObject.SetActive(true);
            alertTimer = alertDuration;
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
            alertCanvas.Rotate(0, 180, 0); // Flip to face camera properly
        }
    }

    // Update the speed display
    void UpdateSpeedDisplay()
    {
        if (speedText != null)
        {
            float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f; // Convert to km/h
            speedText.text = "Speed: " + currentSpeed.ToString("F1") + " km/h"; // Update the text
        }
    }
}

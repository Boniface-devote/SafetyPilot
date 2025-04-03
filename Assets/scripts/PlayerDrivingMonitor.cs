using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
    public Button handbrakeButton;

    public float defaultSpeedLimit = 60f;
    private float speedLimit;
    public float sharpTurnThreshold = 35f;
    public float alertDuration = 2f;
    public int playerScore = 100;

    private float previousSpeed;
    private float alertTimer = 0f;
    private float safeDrivingTimer = 0f;
    private Camera mainCamera;
    private bool isDrivingSafely = true;
    private bool isOnRoad = false;
    private bool canDetect = true;

    void Start()
    {
        mainCamera = Camera.main;
        speedLimit = defaultSpeedLimit;
        if (speedText != null) speedText.text = "Speed: 0 km/h";
        UpdateScoreDisplay();

        if (handbrakeButton != null)
        {
            handbrakeButton.onClick.AddListener(TriggerSuddenBraking);
        }
    }

    void Update()
    {
        MonitorDriving();
        HandleAlertDisplay();
        if (alertCanvas) FaceCamera();
        UpdateSpeedDisplay();
        RewardSafeDriving();
        CheckSuddenBraking();
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

    void ShowAlert(string message, int penalty = 0)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.gameObject.SetActive(true);
            alertTimer = alertDuration;
            if (penalty > 0) ReduceScore(penalty);
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
            if (currentSpeed > 25f)
            {
                ShowAlert("SpeedBump Collision!", 5);
                isDrivingSafely = false;
            }
        }
        else
        {
            ShowAlert("Accident!", 10);
            isDrivingSafely = false;
        }
    }

    void CheckSuddenBraking()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
        if (currentSpeed > 10f && Input.GetKeyDown(KeyCode.LeftControl))
        {
            TriggerSuddenBraking();
        }
    }

    void TriggerSuddenBraking()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
        if (currentSpeed > 10f)
        {
            ShowAlert("Sudden Braking!", 5);
        }
    }

    private IEnumerator DetectionCooldown()
    {
        canDetect = false;
        yield return new WaitForSeconds(1f);
        canDetect = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("UrbanZone") || other.CompareTag("HighwayZone")) && !isOnRoad && canDetect)
        {
            speedLimit = other.CompareTag("UrbanZone") ? 40f : 100f;
            isOnRoad = true;
            ShowAlert(other.tag + ". Speed limit " + speedLimit + " km/h.");
            StartCoroutine(DetectionCooldown());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("UrbanZone") || other.CompareTag("HighwayZone")) && isOnRoad && canDetect)
        {
            speedLimit = defaultSpeedLimit;
            isOnRoad = false;
            ShowAlert("Exited " + other.tag + ". Speed limit default.");
            StartCoroutine(DetectionCooldown());
        }
    }
}

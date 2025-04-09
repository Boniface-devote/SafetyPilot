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

    public float bumpForce = 300f;
    public float speedReductionFactor = 0.5f;
    public float cameraShakeDuration = 0.3f;
    public float cameraShakeMagnitude = 0.1f;
    private Vector3 originalCamPosition;
    private Coroutine shakeCoroutine;

    public AudioSource audioSource;
    public AudioClip overspeedClip;
    public AudioClip bumpClip;
    public AudioClip potholeClip;
    public AudioClip accidentClip;
    public AudioClip suddenBrakeClip;


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

    private bool isOverspeeding = false;
    private bool isSharpTurning = false;

    IEnumerator DelayedMonitorDriving(string message, int penalty, System.Func<bool> condition)
    {
        if (message == "Overspeeding!") isOverspeeding = true;
        if (message == "Sharp Turn!") isSharpTurning = true;

        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        if (condition()) // Check if the condition is still true after the delay
        {
            ShowAlert(message, penalty);
        }

        if (message == "Overspeeding!") isOverspeeding = false;
        if (message == "Sharp Turn!") isSharpTurning = false;
    }


    void MonitorDriving()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
        float steeringAngle = Mathf.Max(Mathf.Abs(frontLeftWheel.steerAngle), Mathf.Abs(frontRightWheel.steerAngle));

        isDrivingSafely = true;

        // Delay overspeeding detection
        if (currentSpeed > speedLimit && !isOverspeeding)
        {
            StartCoroutine(DelayedMonitorDriving("Overspeeding!", 10, () => currentSpeed > speedLimit));
        }

        // Delay sharp turn detection
        if (steeringAngle > sharpTurnThreshold && !isSharpTurning)
        {
            StartCoroutine(DelayedMonitorDriving("Sharp Turn!", 2, () => steeringAngle > sharpTurnThreshold));
        }

        previousSpeed = currentSpeed;
    }


    public void ShowAlert(string message, int penalty = 0)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.gameObject.SetActive(true);
            alertTimer = alertDuration;

            // Play appropriate sound
            switch (message)
            {
                case "Overspeeding!":
                    PlaySound(overspeedClip);
                    break;
                case "Sudden Braking!":
                    PlaySound(suddenBrakeClip);
                    break;
                case "Slow Down on Speed Bumps!":
                    PlaySound(bumpClip);
                    break;
                case "Watch out! You hit a pothole!":
                    PlaySound(potholeClip);
                    break;
                case "Accident!":
                    PlaySound(accidentClip);
                    break;
            }

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

    public void UpdateScoreDisplay()
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
            if (currentSpeed > 18f)
            {
                ShowAlert("Slow Down on Speed Bumps!", 5);
                isDrivingSafely = false;
                ReactToBump(); // Apply force, slow down, shake camera
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
        // Detect Pothole
        if (other.CompareTag("PotHole") && canDetect)
        {
            float currentSpeed = carRigidbody.linearVelocity.magnitude * 3.6f;
            if (currentSpeed > 10f)
            {
                ShowAlert("Watch out! You hit a pothole!", 5);
                ReactToBump(); // Apply force, slow down, shake camera
            }
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
    IEnumerator CameraShake()
    {
        if (mainCamera == null) yield break;

        originalCamPosition = mainCamera.transform.localPosition;

        float elapsed = 0f;
        while (elapsed < cameraShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * cameraShakeMagnitude;
            float y = Random.Range(-1f, 1f) * cameraShakeMagnitude;

            mainCamera.transform.localPosition = originalCamPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalCamPosition;
    }
    void ReactToBump()
    {
        // Add upward force for a jump effect
        carRigidbody.AddForce(Vector3.up * bumpForce);

        // Reduce speed (simulate loss of control)
        carRigidbody.linearVelocity *= speedReductionFactor;

        // Trigger camera shake
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(CameraShake());
    }
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}

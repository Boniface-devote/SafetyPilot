using UnityEngine;
using TMPro;
using GleyTrafficSystem;

public class RoundaboutManager : MonoBehaviour
{
    public Transform[] entryPoints;
    public Transform[] exitPoints;
    public float maxApproachSpeed = 20f;
    public TextMeshProUGUI instructionText;
    public DrivingMonitor drivingMonitor;
    public Rigidbody carRigidbody; // ✅ Assign the player's Rigidbody here in the Inspector

    private bool isInsideRoundabout = false;
    private PlayerCar playerCar;

    private float signalTimer = 0f;
    private bool signalActive = false;
    private float signalDuration = 5f;

    private void Start()
    {
        instructionText.text = "Slow down and signal before entering or exiting.";

        if (drivingMonitor == null)
        {
            drivingMonitor = Object.FindFirstObjectByType<DrivingMonitor>();
        }

        if (carRigidbody == null)
        {
            Debug.LogWarning("Car Rigidbody not assigned in RoundaboutManager!");
        }

        playerCar = Object.FindFirstObjectByType<PlayerCar>();
    }

    private void Update()
    {
        if (playerCar == null) return;

        if (playerCar.IsSignaling())
        {
            if (!signalActive)
            {
                signalActive = true;
                signalTimer = 0f;
            }
            else
            {
                signalTimer += Time.deltaTime;
                if (signalTimer >= signalDuration)
                {
                    playerCar.TurnOffSignals();
                    signalActive = false;
                    signalTimer = 0f;
                }
            }
        }
        else
        {
            signalActive = false;
            signalTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoundAbout") && !isInsideRoundabout)
        {
            isInsideRoundabout = true;

            float currentSpeed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude * 3.6f : 0f;

            if (currentSpeed > maxApproachSpeed)
            {
                drivingMonitor.ShowAlert("You were speeding entering the roundabout!", 5);
                drivingMonitor.playerScore -= 10;
            }
            else
            {
                drivingMonitor.ShowAlert("Good job slowing down!", 3);
                drivingMonitor.playerScore += 5;
            }

            if (!IsSignalUsed())
            {
                drivingMonitor.ShowAlert("You didn’t use your turn signal!", 5);
                drivingMonitor.playerScore -= 10;
            }
            else
            {
                drivingMonitor.ShowAlert("Signaling used correctly at entry!", 3);
                drivingMonitor.playerScore += 5;
            }

            drivingMonitor.UpdateScoreDisplay();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RoundAbout") && isInsideRoundabout)
        {
            isInsideRoundabout = false;

            if (!IsSignalUsed())
            {
                drivingMonitor.ShowAlert("You didn’t use your turn signal!", 5);
                drivingMonitor.playerScore -= 10;
            }
            else
            {
                drivingMonitor.ShowAlert("Good signaling at exit!", 3);
                drivingMonitor.playerScore += 5;
            }

            drivingMonitor.UpdateScoreDisplay();
        }
    }

    private bool IsSignalUsed()
    {
        return playerCar != null && playerCar.IsSignaling();
    }
}

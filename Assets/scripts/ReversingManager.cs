using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ReversingManager : MonoBehaviour
{
    public Transform playerCar;
    public float maxReverseSpeed = 5f;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timerText;

    public float stageTimeLimit = 90f;
    private float remainingTime;

    private bool stageComplete = false;
    private bool hasFailed = false;

    public DrivingMonitor drivingMonitor;

    [Header("Reversing Zones")]
    public List<Collider> reversingZones = new List<Collider>(); // Assign in Inspector
    private bool[] zoneCompleted;

    void Start()
    {
        instructionText.text = "Reverse through all zones accurately.";
        resultText.text = "";
        remainingTime = stageTimeLimit;

        if (drivingMonitor == null)
        {
            drivingMonitor = FindAnyObjectByType<DrivingMonitor>();
        }

        zoneCompleted = new bool[reversingZones.Count];
    }

    void Update()
    {
        // === TIMER ===
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            int secondsLeft = Mathf.FloorToInt(remainingTime);
            timerText.text = "Time Left: " + Mathf.Max(0, secondsLeft) + "s";

            if (remainingTime <= 20f)
                timerText.color = Color.red;
            else if (remainingTime <= stageTimeLimit * 0.5f)
                timerText.color = new Color(1f, 0.65f, 0f);
            else
                timerText.color = Color.green;
        }
        else if (!hasFailed && !stageComplete)
        {
            hasFailed = true;
            ShowResult("Time's up! Press R to restart.", Color.red);
            instructionText.text = "";
            drivingMonitor.ShowAlert("Reversing Failed! Time's up.");
        }

        // === SPEED CHECK ===
        float reverseSpeed = -playerCar.InverseTransformDirection(playerCar.GetComponent<Rigidbody>().linearVelocity).z;
        if (reverseSpeed > maxReverseSpeed)
        {
            drivingMonitor.ShowAlert("Too Fast!", 5);

            if (!hasFailed && !stageComplete)
            {
                hasFailed = true;
                ShowResult("Too fast! Penalty for reckless reversing.", Color.red);
                instructionText.text = "";
            }
        }

        // === OBSTACLE CHECK ===
        if (playerCar.position.y < -1f && !hasFailed)
        {
            hasFailed = true;
            ShowResult("You hit something! Press R to retry.", Color.red);
            instructionText.text = "";
            drivingMonitor.ShowAlert("Obstacle Hit!", 10);
        }

        // === ZONE CHECK ===
        if (!hasFailed && !stageComplete)
        {
            CheckZoneCompletion();

            if (AllZonesCompleted())
            {
                CompleteStage();
            }
        }

        // === RESTART ON "R" ===
        if (Input.anyKeyDown && Input.inputString.ToLower() == "r")
        {
            SceneManager.LoadScene("Reversing");
        }
    }

    void CheckZoneCompletion()
    {
        Bounds carBounds = playerCar.GetComponent<Collider>().bounds;

        for (int i = 0; i < reversingZones.Count; i++)
        {
            if (!zoneCompleted[i] && ZoneContains(reversingZones[i], carBounds))
            {
                zoneCompleted[i] = true;
                drivingMonitor.ShowAlert($"Zone {i + 1} complete!");
                drivingMonitor.playerScore += 5;
                drivingMonitor.UpdateScoreDisplay();
            }
        }
    }

    bool AllZonesCompleted()
    {
        foreach (bool completed in zoneCompleted)
        {
            if (!completed) return false;
        }
        return true;
    }

    bool ZoneContains(Collider zone, Bounds carBounds)
    {
        return zone.bounds.Contains(carBounds.min) && zone.bounds.Contains(carBounds.max);
    }

    void ShowResult(string message, Color color)
    {
        resultText.text = message;
        resultText.color = color;
        StopAllCoroutines();
        StartCoroutine(HideResultAfterDelay(2f));
    }

    IEnumerator HideResultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resultText.text = "";
    }

    public void CompleteStage()
    {
        if (stageComplete || hasFailed) return;

        stageComplete = true;
        ShowResult("Great reversing! All zones complete!", Color.green);
        instructionText.text = "Stage Complete!";

        if (drivingMonitor != null)
        {
            drivingMonitor.ShowAlert("Well Done!");
            drivingMonitor.playerScore += 10;
            drivingMonitor.UpdateScoreDisplay();
        }

        StartCoroutine(LoadNextSceneAfterDelay(3f));
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SampleScene"); // Replace with your actual scene name
    }
}

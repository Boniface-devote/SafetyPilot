using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ObstacleCollisionDetection : MonoBehaviour
{
    private bool canDetect = true;
    public TextMeshProUGUI alertText;
    public GameObject pauseCanvas; // Assign your pause UI Canvas here in the Inspector
    public GameObject speedCanvas; // Assign your speed UI Canvas here in the Inspector

    // Dictionary for tag-based custom messages
    private Dictionary<string, string> tagMessages = new Dictionary<string, string>()
    {
        { "SpeedBump", "Caution: Speed bump ahead.\n\nAction: Gradually reduce your speed to a safe level. Maintain control and avoid braking sharply while driving over the bump." },
        { "PotHole", "Warning: Pothole detected ahead.\n\nAction: Slow down and maneuver carefully to avoid damage to the vehicle." },
        { "Buildings", "Warning: Building ahead.\n\nAction: Stay on the road and avoid collisions with static obstacles." },
        { "salon car", "Watch out: Car ahead.\n\nAction: Keep a safe distance and be prepared for unexpected stops." },
        { "bus", "Caution: Bus ahead.\n\nAction: Watch for sudden stops or passengers boarding/alighting." },
        { "Lorry", "Alert: Heavy vehicle ahead.\n\nAction: Avoid tailgating and allow ample space when overtaking." },
        { "SUV car", "Notice: SUV ahead.\n\nAction: Maintain a safe distance and be alert for road maneuvers." },
        { "taxi", "Alert: Taxi nearby.\n\nAction: Be aware of sudden stops or pickups/drop-offs." },
        { "boda boda", "Caution: Motorcycle ahead.\n\nAction: Share the road safely and be mindful of quick lane changes." }
    };

    private void OnTriggerEnter(Collider other)
    {
        if (canDetect && tagMessages.ContainsKey(other.tag))
        {
            string message = tagMessages[other.tag];
            alertText.text = message;
            Debug.Log(message);

            StartCoroutine(HandleObstaclePause());
        }
    }

    private IEnumerator HandleObstaclePause()
    {
        canDetect = false;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        speedCanvas.SetActive(false);

        yield return new WaitForSecondsRealtime(5f); // Wait while game is paused

        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
        speedCanvas.SetActive(true);

        yield return new WaitForSeconds(3f); // Cooldown before next detection
        canDetect = true;
    }

    private void Start()
    {
        pauseCanvas.SetActive(false);
        speedCanvas.SetActive(true);
    }
}

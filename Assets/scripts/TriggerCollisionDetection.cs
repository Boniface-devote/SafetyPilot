using UnityEngine;
using System.Collections;

public class TriggerCollisionDetection : MonoBehaviour
{
    private bool isOnRoad = false; // Flag to track if the wheels are on the road
    private bool canDetect = true; // Cooldown flag to prevent rapid logging

    private IEnumerator DetectionCooldown()
    {
        canDetect = false;
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        canDetect = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PotHole") && !isOnRoad && canDetect)
        {
            Debug.Log("Wheels are touching the PotHole.");
            isOnRoad = true; // Mark wheels as on the road
            StartCoroutine(DetectionCooldown()); // Start cooldown
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PotHole") && isOnRoad && canDetect)
        {
            Debug.Log("Wheels left the PotHole.");
            isOnRoad = false; // Mark wheels as off the road
            StartCoroutine(DetectionCooldown()); // Start cooldown
        }
    }
}

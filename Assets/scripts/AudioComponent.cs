using UnityEngine;
using DG.Tweening; // Import DOTween for smooth audio transitions

public class AudioComponent : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource engineSound;  // Engine sound of the player's car
    public AudioSource environmentSound; // Background/environment sounds

    [Header("Audio Settings")]
    public float minPitch = 0.8f; // Minimum engine pitch
    public float maxPitch = 2.0f; // Maximum engine pitch
    public float pitchSmoothTime = 0.2f; // Smooth pitch transition time
    public float accelerationThreshold = 0.1f; // Sensitivity for detecting acceleration changes

    private Rigidbody carRigidbody;
    private float previousSpeed;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();

        // Ensure the engine and environment sounds play in a loop
        if (engineSound != null)
        {
            engineSound.loop = true;
            engineSound.Play();
        }

        if (environmentSound != null)
        {
            environmentSound.loop = true;
            environmentSound.Play();
        }
    }

    void Update()
    {
        float currentSpeed = carRigidbody.linearVelocity.magnitude;
        float speedDifference = currentSpeed - previousSpeed;

        // Adjust engine pitch based on speed
        if (engineSound != null)
        {
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, currentSpeed / 50f);
            engineSound.DOPitch(targetPitch, pitchSmoothTime); // Smooth pitch transition
        }

        previousSpeed = currentSpeed;
    }
}

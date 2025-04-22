using GleyTrafficSystem;
using UnityEngine;

public class VehicleLightsComponent : MonoBehaviour
{
    [Tooltip("Blinking interval")]
    public float blinkTime = 0.5f;

    [Tooltip("A GameObject containing all main lights - will be active based on Manager API calls")]
    public GameObject frontLights;

    [Tooltip("Two spotlights that should activate with front lights (optional)")]
    public GameObject spotlightLeft;
    public GameObject spotlightRight;

    [Tooltip("A GameObject containing all reverse lights - will be active if a vehicle is reversing")]
    public GameObject reverseLights;

    [Tooltip("A GameObject containing all rear lights - will be active if main lights are active")]
    public GameObject rearLights;

    [Tooltip("A GameObject containing all brake lights - will be active when a vehicle is braking")]
    public GameObject stopLights;

    [Tooltip("A GameObject containing all blinker left lights - will be active when car turns left")]
    public GameObject blinkerLeft;

    [Tooltip("A GameObject containing all blinker right lights - will be active when car turns right")]
    public GameObject blinkerRight;

    private float currentTime;
    private bool updateLights;
    private bool leftBlink;
    private bool rightBlink;

    public void Initialize()
    {
        currentTime = 0;
        LightsSetup();
    }

    public void DeactivateLights()
    {
        LightsSetup();
        leftBlink = false;
        rightBlink = false;
    }

    private void LightsSetup()
    {
        if (frontLights != null)
            frontLights.SetActive(false);

        if (spotlightLeft != null)
            spotlightLeft.SetActive(false);

        if (spotlightRight != null)
            spotlightRight.SetActive(false);

        if (reverseLights != null)
            reverseLights.SetActive(false);

        if (rearLights != null)
            rearLights.SetActive(false);

        if (stopLights != null)
            stopLights.SetActive(false);

        if (blinkerLeft != null)
        {
            blinkerLeft.SetActive(false);
            updateLights = true;
        }

        if (blinkerRight != null)
        {
            blinkerRight.SetActive(false);
            updateLights = true;
        }
    }

    public void SetBrakeLights(bool active)
    {
        if (stopLights && stopLights.activeSelf != active)
            stopLights.SetActive(active);
    }

    public void SetMainLights(bool active)
    {
        if (frontLights)
            frontLights.SetActive(active);

        if (spotlightLeft)
            spotlightLeft.SetActive(active);

        if (spotlightRight)
            spotlightRight.SetActive(active);

        if (rearLights)
            rearLights.SetActive(active);
    }

    public void SetReverseLights(bool active)
    {
        if (reverseLights && reverseLights.activeSelf != active)
            reverseLights.SetActive(active);
    }

    public void SetBlinker(BlinkType blinkType)
    {
        if (blinkerLeft && blinkerRight)
        {
            switch (blinkType)
            {
                case BlinkType.Stop:
                    leftBlink = false;
                    rightBlink = false;
                    break;
                case BlinkType.BlinkLeft:
                    leftBlink = true;
                    rightBlink = false;
                    break;
                case BlinkType.BlinkRight:
                    rightBlink = true;
                    leftBlink = false;
                    break;
            }
        }
    }

    public void UpdateLights(float realtimeSinceStartup)
    {
        if (!updateLights) return;

        if (realtimeSinceStartup - currentTime > blinkTime)
        {
            currentTime = realtimeSinceStartup;

            if (blinkerLeft)
            {
                if (!leftBlink && blinkerLeft.activeSelf)
                    blinkerLeft.SetActive(false);
                else if (leftBlink)
                    blinkerLeft.SetActive(!blinkerLeft.activeSelf);
            }

            if (blinkerRight)
            {
                if (!rightBlink && blinkerRight.activeSelf)
                    blinkerRight.SetActive(false);
                else if (rightBlink)
                    blinkerRight.SetActive(!blinkerRight.activeSelf);
            }
        }
    }
}

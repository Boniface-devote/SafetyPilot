using UnityEngine;
using UnityEngine.UI; // Required for UI.Button
using System.Collections.Generic;

namespace GleyTrafficSystem
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    public class PlayerCar : MonoBehaviour
    {
        public List<AxleInfo> axleInfos;
        public Transform centerOfMass;
        public float maxMotorTorque;
        public float maxSteeringAngle;
        public float handbrakeTorque = 3000f; // Handbrake strength
        public float steeringSmoothingSpeed = 5f; // New variable for steering smoothing

        VehicleLightsComponent lightsComponent;
        bool mainLights;
        bool brake;
        bool reverse;
        bool blinkLeft;
        bool blinkRight; // Fixed typo
        bool handbrakeActive = false;

        float realtimeSinceStartup;
        Rigidbody rb;
        UIInput inputScript;

        // Reference to UI buttons for touch controls
        public Button leftTurnSignalButton;
        public Button rightTurnSignalButton;
        public Button handbrake;
        public Button FrontLights;

        private float currentSteeringAngle = 0f; // Current steering angle

        private void Start()
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
            inputScript = gameObject.AddComponent<UIInput>().Initializ();
            lightsComponent = gameObject.GetComponent<VehicleLightsComponent>();
            lightsComponent.Initialize();
            rb = GetComponent<Rigidbody>();

            // Add listeners for touch controls
            if (leftTurnSignalButton != null)
            {
                leftTurnSignalButton.onClick.AddListener(OnLeftTurnSignalClicked);
            }
            if (rightTurnSignalButton != null)
            {
                rightTurnSignalButton.onClick.AddListener(OnRightTurnSignalClicked);
            }
            if (handbrake != null)
            {
                handbrake.onClick.AddListener(OnhandbrakeClicked);
            }
            if (FrontLights != null)
            {
                FrontLights.onClick.AddListener(OnFrontLightsClicked);
            }
        }

        // Method to handle left turn signal button click
        private void OnLeftTurnSignalClicked()
        {
            blinkLeft = !blinkLeft;
            if (blinkLeft)
            {
                blinkRight = false;
                lightsComponent.SetBlinker(BlinkType.BlinkLeft);
            }
            else
            {
                lightsComponent.SetBlinker(BlinkType.Stop);
            }
        }

        // Method to handle right turn signal button click
        private void OnRightTurnSignalClicked()
        {
            blinkRight = !blinkRight;
            if (blinkRight)
            {
                blinkLeft = false;
                lightsComponent.SetBlinker(BlinkType.BlinkRight);
            }
            else
            {
                lightsComponent.SetBlinker(BlinkType.Stop);
            }
        }
        // Method to handle handbrake
        private void OnhandbrakeClicked()
        {
            handbrakeActive = !handbrakeActive;
        }
        // Method to handle lights on and off
        private void OnFrontLightsClicked()
        {
            mainLights = !mainLights;
            lightsComponent.SetMainLights(mainLights);
        }

        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }
            Transform visualWheel = collider.transform.GetChild(0);
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        public void FixedUpdate()
        {
            float motor = handbrakeActive ? 0 : maxMotorTorque * inputScript.GetVerticalInput();
            float targetSteering = maxSteeringAngle * inputScript.GetHorizontalInput();

            // Smoothly adjust the steering angle
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteering, Time.fixedDeltaTime * steeringSmoothingSpeed);

            float localVelocity = transform.InverseTransformDirection(rb.linearVelocity).z + 0.1f;
            reverse = false;
            brake = false;
            if (localVelocity < 0) reverse = true;

            if (motor < 0 && localVelocity > 0 || motor > 0 && localVelocity < 0)
            {
                brake = true;
            }

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = currentSteeringAngle;
                    axleInfo.rightWheel.steerAngle = currentSteeringAngle;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                if (handbrakeActive)
                {
                    axleInfo.leftWheel.brakeTorque = handbrakeTorque;
                    axleInfo.rightWheel.brakeTorque = handbrakeTorque;
                }
                else
                {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                }
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }

        private void Update()
        {
            realtimeSinceStartup += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                handbrakeActive = !handbrakeActive;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                mainLights = !mainLights;
                lightsComponent.SetMainLights(mainLights);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                blinkLeft = !blinkLeft;
                if (blinkLeft)
                {
                    blinkRight = false;
                    lightsComponent.SetBlinker(BlinkType.BlinkLeft);
                }
                else
                {
                    lightsComponent.SetBlinker(BlinkType.Stop);
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                blinkRight = !blinkRight;
                if (blinkRight)
                {
                    blinkLeft = false;
                    lightsComponent.SetBlinker(BlinkType.BlinkRight);
                }
                else
                {
                    lightsComponent.SetBlinker(BlinkType.Stop);
                }
            }

            lightsComponent.SetBrakeLights(brake || handbrakeActive);
            lightsComponent.SetReverseLights(reverse);
            lightsComponent.UpdateLights(realtimeSinceStartup);
        }
    }
}
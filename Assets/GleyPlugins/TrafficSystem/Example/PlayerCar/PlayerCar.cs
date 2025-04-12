using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

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
        public float handbrakeTorque = 3000f;
        public float steeringSmoothingSpeed = 5f;

        VehicleLightsComponent lightsComponent;
        bool mainLights;
        bool brake;
        bool reverse;
        bool blinkLeft;
        bool blinkRight;
        bool handbrakeActive = false;

        float realtimeSinceStartup;
        Rigidbody rb;
        UIInput inputScript;

        public Button leftTurnSignalButton;
        public Button rightTurnSignalButton;
        public Button handbrake;
        public Button FrontLights;

        public Button gearToggleButton;      // NEW: Optional UI button for toggling gear
        public TextMeshProUGUI gearDisplay;             // NEW: Optional UI Text to show gear (D/R)

        private float currentSteeringAngle = 0f;
        private bool isInReverseGear = false; // NEW: Track current gear

        private void Start()
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
            inputScript = gameObject.AddComponent<UIInput>().Initializ();
            lightsComponent = gameObject.GetComponent<VehicleLightsComponent>();
            lightsComponent.Initialize();
            rb = GetComponent<Rigidbody>();

            // Apply lower grip if RainyWeather scene is loaded
            if (SceneManager.GetActiveScene().name == "RainyWeather")
            {
                ApplyRainyWeatherGrip();
            }

            if (leftTurnSignalButton != null)
                leftTurnSignalButton.onClick.AddListener(OnLeftTurnSignalClicked);
            if (rightTurnSignalButton != null)
                rightTurnSignalButton.onClick.AddListener(OnRightTurnSignalClicked);
            if (handbrake != null)
                handbrake.onClick.AddListener(OnhandbrakeClicked);
            if (FrontLights != null)
                FrontLights.onClick.AddListener(OnFrontLightsClicked);
            if (gearToggleButton != null)
                gearToggleButton.onClick.AddListener(ToggleGear);
        }

        private void ApplyRainyWeatherGrip()
        {
            foreach (AxleInfo axleInfo in axleInfos)
            {
                AdjustFriction(axleInfo.leftWheel, 0.5f, 0.6f);
                AdjustFriction(axleInfo.rightWheel, 0.5f, 0.6f);
            }
        }

        public bool IsSignaling()
        {
            return blinkLeft || blinkRight;
        }

        private void AdjustFriction(WheelCollider wheel, float forwardStiffness, float sidewaysStiffness)
        {
            WheelFrictionCurve forwardFriction = wheel.forwardFriction;
            forwardFriction.stiffness = forwardStiffness;
            wheel.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wheel.sidewaysFriction;
            sidewaysFriction.stiffness = sidewaysStiffness;
            wheel.sidewaysFriction = sidewaysFriction;
        }

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

        private void OnhandbrakeClicked()
        {
            handbrakeActive = !handbrakeActive;
        }

        private void OnFrontLightsClicked()
        {
            mainLights = !mainLights;
            lightsComponent.SetMainLights(mainLights);
        }

        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0) return;
            Transform visualWheel = collider.transform.GetChild(0);
            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);
            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        public void FixedUpdate()
        {
            float verticalInput = inputScript.GetVerticalInput(); // W/S or Up/Down
            float motor = 0f;

            float targetSteering = maxSteeringAngle * inputScript.GetHorizontalInput();
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteering, Time.fixedDeltaTime * steeringSmoothingSpeed);

            float localVelocityZ = transform.InverseTransformDirection(rb.linearVelocity).z;
            brake = false;

            if (!handbrakeActive)
            {
                if (verticalInput > 0)
                {
                    // Drive forward (or backward if in reverse gear)
                    motor = maxMotorTorque * (isInReverseGear ? -1 : 1);
                }
                else
                {
                    // Apply braking if DownArrow/S is pressed
                    brake = verticalInput < 0;
                }
            }

            reverse = localVelocityZ < -0.1f;

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

                float appliedBrakeTorque = 0f;
                if (handbrakeActive)
                    appliedBrakeTorque = handbrakeTorque;
                else if (brake)
                    appliedBrakeTorque = handbrakeTorque * 0.5f; // normal brake force

                axleInfo.leftWheel.brakeTorque = appliedBrakeTorque;
                axleInfo.rightWheel.brakeTorque = appliedBrakeTorque;

                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }


        private void Update()
        {
            realtimeSinceStartup += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.LeftControl))
                handbrakeActive = !handbrakeActive;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                mainLights = !mainLights;
                lightsComponent.SetMainLights(mainLights);
            }

            if (Input.GetKeyDown(KeyCode.Q))
                OnLeftTurnSignalClicked();

            if (Input.GetKeyDown(KeyCode.E))
                OnRightTurnSignalClicked();

            if (Input.GetKeyDown(KeyCode.G))
                ToggleGear();

            lightsComponent.SetBrakeLights(brake || handbrakeActive);
            lightsComponent.SetReverseLights(reverse);
            lightsComponent.UpdateLights(realtimeSinceStartup);

            if (gearDisplay != null)
                gearDisplay.text = isInReverseGear ? "R" : "D";
        }

        public void ToggleGear()
        {
            isInReverseGear = !isInReverseGear;
            Debug.Log("Gear: " + (isInReverseGear ? "Reverse" : "Drive"));
        }

        public void TurnOffSignals()
        {
            blinkLeft = false;
            blinkRight = false;
            lightsComponent.SetBlinker(BlinkType.Stop);
        }
    }
}

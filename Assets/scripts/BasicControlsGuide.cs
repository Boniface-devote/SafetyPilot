using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BasicControlsGuide : MonoBehaviour
{
    [System.Serializable]
    public class ControlStep
    {
        public string instructionText;
        public AudioClip instructionAudio;
        public string expectedAction;
        public int reward = 5;
        public int penalty = 3;
    }

    public List<ControlStep> steps;
    public AudioSource audioSource;
    public DrivingMonitor drivingMonitor;
    public float timeoutDuration = 8f;
    public TMP_Text instructionUIText;

    private int currentStepIndex = 0;
    private bool awaitingInput = false;
    private float stepTimer = 0f;
    private bool tutorialCompleted = false;
    private float postTutorialTimer = 0f;

    private Dictionary<string, bool> inputFlags = new Dictionary<string, bool>();

    void Start()
    {
        if (steps.Count > 0)
            StartCoroutine(PlayNextStep());
    }

    void Update()
    {
        if (tutorialCompleted)
        {
            postTutorialTimer += Time.deltaTime;
            if (postTutorialTimer >= 180f) // 3 minutes
            {
                SceneManager.LoadScene("SampleScene");
            }
            return;
        }

        if (!awaitingInput) return;

        stepTimer += Time.deltaTime;

        if (inputFlags.ContainsKey(steps[currentStepIndex].expectedAction) && inputFlags[steps[currentStepIndex].expectedAction])
        {
            RewardPlayer();
        }

        if (stepTimer >= timeoutDuration)
        {
            PenalizePlayer();
        }

        DetectKeyboardInputs();
    }

    public void RegisterAction(string actionName)
    {
        if (awaitingInput && actionName == steps[currentStepIndex].expectedAction)
        {
            inputFlags[actionName] = true;
        }
    }

    private IEnumerator PlayNextStep()
    {
        awaitingInput = false;
        stepTimer = 0f;
        inputFlags.Clear();

        if (currentStepIndex >= steps.Count)
        {
            Debug.Log("Tutorial complete!");
            tutorialCompleted = true;

            if (instructionUIText != null)
                instructionUIText.text = "Tutorial complete! You may now drive freely.";

            yield break;
        }

        ControlStep step = steps[currentStepIndex];

        if (instructionUIText != null)
        {
            instructionUIText.text = step.instructionText;
        }

        if (step.instructionAudio != null && audioSource != null)
        {
            audioSource.PlayOneShot(step.instructionAudio);
            yield return new WaitForSeconds(step.instructionAudio.length);
        }

        awaitingInput = true;
        stepTimer = 0f;
    }

    private void RewardPlayer()
    {
        awaitingInput = false;
        Debug.Log("Correct action!");
        drivingMonitor.playerScore += steps[currentStepIndex].reward;
        drivingMonitor.UpdateScoreDisplay();
        currentStepIndex++;
        StartCoroutine(PlayNextStep());
    }

    private void PenalizePlayer()
    {
        awaitingInput = false;
        Debug.Log("Action timed out!");
        drivingMonitor.playerScore -= steps[currentStepIndex].penalty;
        if (drivingMonitor.playerScore < 0) drivingMonitor.playerScore = 0;
        drivingMonitor.UpdateScoreDisplay();
        currentStepIndex++;
        StartCoroutine(PlayNextStep());
    }

    private void DetectKeyboardInputs()
    {
        if (currentStepIndex >= steps.Count) return;

        string expected = steps[currentStepIndex].expectedAction;

        switch (expected)
        {
            case "Brake":
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    RegisterAction("Brake");
                break;
            case "Accelerate":
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    RegisterAction("Accelerate");
                break;
            case "TurnLeft":
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    RegisterAction("TurnLeft");
                break;
            case "TurnRight":
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    RegisterAction("TurnRight");
                break;
            case "Handbrake":
                if (Input.GetKeyDown(KeyCode.LeftControl))
                    RegisterAction("Handbrake");
                break;
            case "Gearshift":
                if (Input.GetKeyDown(KeyCode.G))
                    RegisterAction("Gearshift");
                break;
            case "CameraChange":
                if (Input.GetKeyDown(KeyCode.C))
                    RegisterAction("CameraChange");
                break;
            case "LeftTurnSignal":
                if (Input.GetKeyDown(KeyCode.Q))
                    RegisterAction("LeftTurnSignal");
                break;
            case "RightTurnSignal":
                if (Input.GetKeyDown(KeyCode.E))
                    RegisterAction("RightTurnSignal");
                break;
            case "Lights":
                if (Input.GetKeyDown(KeyCode.Space))
                    RegisterAction("Lights");
                break;
        }
    }
}

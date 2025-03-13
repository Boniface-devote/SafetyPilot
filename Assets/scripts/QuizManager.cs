using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public Image questionImage;
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text highScoreText;
    public Button nextButton;
    public Button exitButton;

    [Header("Quiz Data")]
    public List<QuestionData> questions;
    private QuestionData currentQuestion;
    private int score = 0;
    private int highScore = 0;
    private List<int> questionIndices;
    private int questionIndex = 0;

    [Header("Timer Settings")]
    public float timePerQuestion = 10f;
    private float timer;
    private bool isTimerRunning = false;

    [Header("Feedback Colors")]
    public Color correctColor = Color.green;
    public Color incorrectColor = Color.red;
    public Color defaultColor = Color.white;

    void Start()
    {
        LoadHighScore();
        ShuffleQuestions();
        LoadNextQuestion();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                isTimerRunning = false;
                timerText.text = "Time's Up!";
                HighlightCorrectAnswer();
                StartCoroutine(NextQuestionAfterDelay());
            }
        }
    }

    void ShuffleQuestions()
    {
        questionIndices = new List<int>();
        for (int i = 0; i < questions.Count; i++)
        {
            questionIndices.Add(i);
        }
        questionIndices.Sort((a, b) => Random.Range(-1, 2));
    }

    void LoadNextQuestion()
    {
        if (questionIndex < questionIndices.Count)
        {
            currentQuestion = questions[questionIndices[questionIndex]];
            questionText.text = currentQuestion.question;
            questionImage.sprite = currentQuestion.questionSprite;

            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerTexts[i].text = currentQuestion.answers[i];
                answerButtons[i].onClick.RemoveAllListeners();
                int index = i;
                answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
                answerButtons[i].GetComponent<Image>().color = defaultColor; // Reset button color
            }

            timer = timePerQuestion;
            isTimerRunning = true;
            questionIndex++;
        }
        else
        {
            EndQuiz();
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        isTimerRunning = false;
        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            score++;
            answerButtons[selectedIndex].GetComponent<Image>().color = correctColor;
        }
        else
        {
            answerButtons[selectedIndex].GetComponent<Image>().color = incorrectColor;
            HighlightCorrectAnswer();
        }

        scoreText.text = "Score: " + score;
        StartCoroutine(NextQuestionAfterDelay());
    }

    void HighlightCorrectAnswer()
    {
        answerButtons[currentQuestion.correctAnswerIndex].GetComponent<Image>().color = correctColor;
    }

    IEnumerator NextQuestionAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        OnNextButton();
    }

    public void OnNextButton()
    {
        nextButton.gameObject.SetActive(false);
        LoadNextQuestion();
    }

    public void ExitQuiz()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void EndQuiz()
    {
        questionText.text = "Quiz Over! Final Score: " + score;
        questionImage.gameObject.SetActive(false);
        foreach (var btn in answerButtons)
        {
            btn.gameObject.SetActive(false);
        }
        foreach (var txt in answerTexts)
        {
            txt.gameObject.SetActive(false);
        }
        nextButton.gameObject.SetActive(false);
        SaveHighScore();
    }

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }

    void SaveHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}

[System.Serializable]
public class QuestionData
{
    public string question;
    public Sprite questionSprite;
    public string[] answers;
    public int correctAnswerIndex;
}

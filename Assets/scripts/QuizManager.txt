using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public Image questionImage;
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;
    public TMP_Text scoreText;
    public Button nextButton;
    public Button exitButton;

    [Header("Quiz Data")]
    public List<QuestionData> questions;
    private QuestionData currentQuestion;
    private int score = 0;
    private List<int> questionIndices;
    private int questionIndex = 0;

    void Start()
    {
        ShuffleQuestions();
        LoadNextQuestion();
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
            }

            questionIndex++;
        }
        else
        {
            EndQuiz();
        }
    }

    void CheckAnswer(int selectedIndex)
    {
        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            score++;
        }

        scoreText.text = "Score: " + score;
        nextButton.gameObject.SetActive(true);
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

        //exitButton.gameObject.SetActive(true);


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

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void ExitButton()
    {
        Application.Quit();
        Debug.Log("game exit");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("City");
    }
    public void StartQuiz()
    {
        SceneManager.LoadScene("quiz");
    }
}

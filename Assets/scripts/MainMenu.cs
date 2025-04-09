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
    public void StartBasicLession()
    {
        SceneManager.LoadScene("BasicControls");
    }
    public void StartIntersection()
    {
        SceneManager.LoadScene("Intersection");
    }
    public void StartDrivingZone()
    {
        SceneManager.LoadScene("DrivingZone");
    }
    public void StartReversing()
    {
        SceneManager.LoadScene("Reversing");
    }
    public void StartParking()
    {
        SceneManager.LoadScene("Parking");
    }
    public void StartRoundAbout()
    {
        SceneManager.LoadScene("RoundAbout");
    }
    public void StartRainyWeather()
    {
        SceneManager.LoadScene("RainyWeather");
    }
}

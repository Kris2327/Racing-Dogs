using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    //describe variables
    [Header("SignUp")]
    public GameObject car;
    public GameObject SignUpPage;
    public Slider age;
    public TextMeshProUGUI username;
    public TextMeshProUGUI ageValue;

    void Start()
    {
        if (PlayerPrefs.GetFloat("openedPage") != 1)
        {
            SignUpPage.SetActive(true);
            car.SetActive(false);
        } else
        {
            car.SetActive(true);
            SignUpPage.SetActive(false);
        }

        PlayerPrefs.SetString("game", "should play");
    }

    void Update()
    {
        
    }

    //continue button click
    public void ContinueButton()
    {
        PlayerPrefs.SetString("username", username.text);
        PlayerPrefs.SetFloat("age", age.value);

        SignUpPage.SetActive(false);
        car.SetActive(true);
    }

    //slider value
    public void SliderValue()
    {
        ageValue.text = age.value.ToString();
    }

    //start button click
    public void StartButon()
    {
        if (PlayerPrefs.GetFloat("openedPage") == 1)
        {
            SceneManager.LoadScene("MainPage");
        } else
        {
            SceneManager.LoadScene("Tutorial");
        }

    }

    //exit button click
    public void ExitButton()
    {
        Application.Quit();
    }
}
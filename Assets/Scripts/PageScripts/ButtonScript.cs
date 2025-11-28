using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
	public void OpenGarage()
	{
		SceneManager.LoadScene("GarageScene");
	}

	public void MainPage()
	{
		SceneManager.LoadScene("MainPage");

		PlayerPrefs.SetString("game", "played");
	}

	public void Retry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Chapters()
	{
		SceneManager.LoadScene("ChaptersScene");
	}

	public void DogUpgrades()
	{
		SceneManager.LoadScene("DogUpgrades");
	}
}

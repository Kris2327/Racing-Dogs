using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class MainPage : MonoBehaviour
{
    [Header("Cars")]
    public Rigidbody rb;
    public LayerMask whatisground;
    public float carHeight;

    [Header("Canvas")]
    public GameObject canvas;
    public GameObject Skipbtn;
    public TextMeshProUGUI timer_text;
    public TextMeshProUGUI fuel_text;
    public GameObject Gas;

    private bool grounded;
    private bool started = true;

    private double time_left;
    private int minutes;
    private int seconds;
    private string connection;

    void Start()
    {
        canvas.SetActive(false);

        if (PlayerPrefs.GetString("game") != "played")
        {
			StartCoroutine(Wait());
			StartCoroutine(Wait2());
		} else
        {
            canvas.SetActive(true);
            Skipbtn.SetActive(false);
        }

		if (Application.platform == RuntimePlatform.Android)
		{
			var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "fuel.db");

			if (!File.Exists(filepath))
			{
				var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "fuel.db");
				while (!loadDb.isDone) { }

				File.WriteAllBytes(filepath, loadDb.bytes);
			}

			//open db connection
			connection = "URI=file:" + filepath;
		}
		else
		{
			var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "fuel.db");

			//open db connection
			connection = "URI=file:" + dbPath;
		}
	}

    void Update()
    {
		grounded = Physics.Raycast(transform.position, Vector3.down, carHeight * 0.5f + 0.2f, whatisground);

        if (!started)
        {
            CarClass car = new CarClass();

            car.MoveForward(rb, transform, started, grounded, 350f, 100f);
        }

        if (transform.gameObject.name == "firstCar")
        {
			Fuel f = new Fuel();

			time_left = f.Get_Time(connection);
            time_left -= Time.deltaTime;
			minutes = Mathf.FloorToInt((int)time_left / 60);
			seconds = Mathf.FloorToInt((int)time_left % 60);

			timer_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            f.datasource = f.Load_Database(connection);
            fuel_text.text = f.Load_Fuel(f.datasource).ToString();
		}
	}

    public void ShowGas()
    {
        if (Gas.activeSelf)
        {
            Gas.SetActive(false);
        } else
        {
            Gas.SetActive(true);
        }
    }

	/*private void OnApplicationQuit()
	{
        if (transform.gameObject.name == "firstCar")
        {
            string[] array = new string[3];

            array[0] = fuel_text.text;
            array[1] = DateTime.Now.ToString("H:mm");
            array[2] = time_left.ToString();

            Fuel f = new Fuel();
            f.Save_Database(connection, array);
			PlayerPrefs.SetString("date", f.CheckDate());
		}
	}*/

	//if the skip button is clicked
	public void SkipBtn()
    {
        canvas.SetActive(true);
        Skipbtn.SetActive(false);
    }

    //wait to start the cars
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);

        started = false;
        Skipbtn.SetActive(true);
    }

    //wait for the cars to finish
    IEnumerator Wait2()
    {
        yield return new WaitForSeconds(7);

        canvas.SetActive(true);

        PlayerPrefs.SetString("game", "played");
    }
}
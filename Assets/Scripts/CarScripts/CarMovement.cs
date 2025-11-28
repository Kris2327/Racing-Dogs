using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CarMovement : MonoBehaviour
{
    //public variables
    [Header("About car")]
    public Rigidbody carRb;
    public float carHeight;
    public LayerMask whatisground;
    public GameObject[] particles;
    public TrailRenderer[] trails;
	public Image health;

    private float speed;
    private float aceleration;
    private float superPowerValue = 20f;
	private float driftSpeed;

    [Header("Race")]
    public TextMeshProUGUI StartTimer;
	public TextMeshProUGUI Timer;
	public GameObject end;
	public TextMeshProUGUI treatsText;
	public Image[] missions;
	public TextMeshProUGUI[] missionsDescription;
	private float[] missionsText = new float[3];
	public Sprite success;
	public Sprite failure;
	public GameObject bonus;

    [Header("SuperPower")]
    public Image superPowerImage;
    public ParticleSystem superPowerParticle;

	[Header("Pause")]
	public GameObject PauseMenu;
	public GameObject cube;
	public TextMeshProUGUI finish_time;

	[Header("Variables")]
	public bool ismovingR;
	public bool ismovingL;

	[Header("Obsticals")]
	public GameObject obstaiclas_levels;
	public GameObject[] short_cuts;

	float speedR;
	float speedL;
	float HandleUpgrades = 0;

	//private variables
	bool grounded;
    bool started = false;
    int cooldown = 3;
    bool isSPClicked = false;
    bool isfinished = false;
	float startTime;
	string connection;
	string connection2;
	int carNumber = 1;
	float RealTime;
	float pauseTime;
	int level = 0;
	int treats = 0;
	int RealTimeEstimated;

	//money
	int money;

	//collect all data sources
	private string[][] dataSources = new string[2][];

	void Start()
    {
		Permission.RequestUserPermission(Permission.ExternalStorageWrite);

		if (Application.platform == RuntimePlatform.Android)
		{
			var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "Car1Upgrades.db");

			if (!File.Exists(filepath))
			{
				var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "Car1Upgrades.db");
				while (!loadDb.isDone) { }

				File.WriteAllBytes(filepath, loadDb.bytes);
			}

			//open db connection
			connection = "URI=file:" + filepath;
		}
		else
		{
			var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "Car1Upgrades.db");

			//open db connection
			connection = "URI=file:" + dbPath;
		}

		if (Application.platform == RuntimePlatform.Android)
		{
			var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "Levels.db");

			if (!File.Exists(filepath))
			{
				var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "Levels.db");
				while (!loadDb.isDone) { }

				File.WriteAllBytes(filepath, loadDb.bytes);
			}

			//open db connection
			connection2 = "URI=file:" + filepath;
		}
		else
		{
			var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "Levels.db");

			//open db connection
			connection2 = "URI=file:" + dbPath;
		}

		LoadUpgardes();
		LoadMoney();

		if (transform.gameObject.name == "firstCar" || carNumber == 1)
        {
            FirstCar f = new FirstCar();

            speed = f.speed + 25 * Convert.ToInt16(dataSources[carNumber - 1][2]);
            aceleration = f.acerelation + 25 * Convert.ToInt16(dataSources[carNumber - 1][1]);
			driftSpeed = f.driftSpeed + 25 * Convert.ToInt16(dataSources[carNumber - 1][2]);
			superPowerValue = 25 + (Convert.ToInt16(dataSources[carNumber - 1][8]) * 5);
        }

        superPowerParticle.Stop();

        StartTimer.text = cooldown.ToString();

        StartCoroutine(Wait());

        for (int i = 0; i < trails.Length; i++)
        {
            trails[i].emitting = false;
        }

		for (int i = 0; i < missions.Length; i++)
		{
			missions[i].sprite = failure;
			missionsDescription[i].gameObject.SetActive(false);
		}

		level = PlayerPrefs.GetInt("level");

		if (level == 4)
		{
			RealTimeEstimated = 100;
			obstaiclas_levels.SetActive(true);
		}

		if (level == 8)
		{
			RealTimeEstimated = 90;
			obstaiclas_levels.SetActive(true);
		}

		if (level == 7 || level == 3 || level == 8)
		{
			short_cuts[1].SetActive(true); short_cuts[0].SetActive(false);
		}
		else
		{
			short_cuts[0].SetActive(true); short_cuts[1].SetActive(false);
		}

		if (level == 9) RealTimeEstimated = 30; bonus.SetActive(true);
	}

    void Update()
    {
		grounded = Physics.Raycast(transform.position, Vector3.down, carHeight * 0.5f + 0.2f, whatisground);

		//move forward
        if (!isSPClicked && started && !PauseMenu.activeSelf)
        {
			CarClass carsMovement = new CarClass();

			if (SceneManager.GetActiveScene().name == "snowmap")
			{
				if (ismovingL || ismovingR)
				{
					carsMovement.MoveForward(carRb, transform, isfinished, grounded, driftSpeed + 50f, aceleration + 20f);
				}
				else
				{
					carsMovement.MoveForward(carRb, transform, isfinished, grounded, speed + 50f, aceleration + 20f);
				}
			}
			else
			{
				if (ismovingL || ismovingR)
				{
					carsMovement.MoveForward(carRb, transform, isfinished, grounded, driftSpeed, aceleration);
				}
				else
				{
					carsMovement.MoveForward(carRb, transform, isfinished, grounded, speed, aceleration);
				}
			}
		} else if (isSPClicked && superPowerImage.fillAmount > 0 && started && !PauseMenu.activeSelf)
        {
			if (SceneManager.GetActiveScene().name != "snowmap")
			{
				CarClass carsMovement = new CarClass();

				carsMovement.MoveForward(carRb, transform, isfinished, grounded, speed + 80 + (superPowerValue / 10) * 5, aceleration);
			}
			else
			{
				CarClass carsMovement = new CarClass();

				carsMovement.MoveForward(carRb, transform, isfinished, grounded, speed + 130 + (superPowerValue / 10) * 5, aceleration + 20);
			}

			superPowerImage.fillAmount -= (0.3f) * Time.deltaTime;
		}
        else if (superPowerImage.fillAmount <= 0)
        {
            isSPClicked = false;

            superPowerImage.fillAmount = 0;
            superPowerParticle.Stop();
        }

		//rotate right
		if (ismovingL && started)
		{
            FirstCar firstCar = new FirstCar();
            CarClass carClass = new CarClass();

			if (speedR < 140f + HandleUpgrades * 5)
			{
				speedR += (0.1f + HandleUpgrades) * Time.deltaTime;
			}
			else
			{
				speedR = firstCar.handle;
			}

			carClass.RotateRight(transform, speedR);

			foreach (TrailRenderer t in trails)
			{
				t.emitting = true;
			}
		}
		else
		{
			FirstCar firstCar = new FirstCar();
			speedR = firstCar.handle;
		}

		//rotate left
		if (ismovingR && started)
		{
			FirstCar firstCar = new FirstCar();
			CarClass carClass = new CarClass();

			if (speedL < 140f + HandleUpgrades * 5)
			{
				speedL += (0.1f + HandleUpgrades) * Time.deltaTime;
			}
			else
			{
				speedL = firstCar.handle;
			}

			carClass.RotateLeft(transform, speedL);

			foreach (TrailRenderer t in trails)
			{
				t.emitting = true;
			}
		}
		else
		{
			FirstCar firstCar = new FirstCar();
			speedL = firstCar.handle;
		}

		//check rotation
		if (grounded)
		{
			if (transform.rotation != Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z))
			{
				transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);
			}

			if (health.fillAmount <= 0)
			{
				started = false;
			}
		}

		//checking the time
		SetTime();
	}

	public void RotateRight(bool _move)
	{
		ismovingR = _move;

		if (ismovingR == false)
		{
			foreach (TrailRenderer t in trails)
			{
				t.emitting = false;
			}
		}
	}

	public void RotateLeft(bool _move)
	{
		ismovingL = _move;

		if (ismovingL == false)
		{
			foreach (TrailRenderer t in trails)
			{
				t.emitting = false;
			}
		}
	}

    // click superpower button
    public void SuperPowerClick()
    {
        if (started)
        {
			isSPClicked = true;
			superPowerParticle.Play();
		}
    }

	//pause click
	public void PauseClick()
	{
		PauseMenu.SetActive(true);
	}

	//resume click
	public void ResumeClick()
	{
		PauseMenu.SetActive(false);
	}

	//ontrigger enter function
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Treats")
		{
			if (bonus.activeSelf) treats += 6; else treats += 2;

			Destroy(other.gameObject);
		}

		if (other.tag == "finish")
		{
			cube.SetActive(false);
			CheckScore();
			StartCoroutine(Finish());
			UpdateLevels();
		}
	}

	//time function
	private void SetTime()
	{
		if (!isfinished && started)
		{
			if (PauseMenu.activeSelf)
			{
				pauseTime = (Time.time - RealTime) - startTime;
			}
			else
			{
				RealTime = (Time.time - startTime) - pauseTime;
			}

			if (level == 4 || level == 8 || level == 9)
			{
				Timer.text = "Time: " + String.Format("{0:00:00}", (RealTimeEstimated - RealTime) * 100);

				if ((RealTimeEstimated - RealTime) * 100 <= 0 && !isfinished)
				{
					Timer.text = "Time: 0";

					cube.SetActive(false);
					CheckScore();
					StartCoroutine(Finish());
					UpdateLevels();
				}
			} else
			{
				Timer.text = "Time: " + String.Format("{0:00:00}", RealTime * 100);
			}
		}
	}
	//check score
	private void CheckScore()
	{
		int score = 0;

		//check the tutorial map
		if (SceneManager.GetActiveScene().name == "Tutorial")
		{
			missionsText[0] = 100;
			missionsText[1] = 80;
			missionsText[2] = 60;

			for (int i = 0; i < missions.Length; i++)
			{
				if (Convert.ToInt16((Time.time - startTime) - pauseTime) <= Convert.ToInt16(missionsText[i]))
				{
					missions[i].sprite = success;

					money += Convert.ToInt16(missionsText[i]);
				}
			}
		}

		//check the river map
		if (SceneManager.GetActiveScene().name == "RiverRace")
		{
			missionsText[0] = 120;
			missionsText[1] = 100;
			missionsText[2] = 80;

			missionsDescription[0].text = "1 place";
			missionsDescription[1].text = "100:00";
			missionsDescription[2].text = "120:00";
		
			if (transform.gameObject.name != "unfinished")
			{
				score++;
			}

			for (int i = 1; i < 3; i++)
			{
				if (Convert.ToInt16((Time.time - startTime) - pauseTime) <= Convert.ToInt16(missionsDescription[i].text.Replace(":00", "")))
				{
					score++;
				}
			}

			if (score >= 3)
			{
				money = 300;
			} else
			{
				if (score == 2) money = 200;
				if (score == 1) money = 100;
			}

			if (level == 9) money = 0;
			if (level == 10 && transform.gameObject.name != "unfinished") score = 3; money = 400; 

			money += treats;

			for (int i = 0; i < score; i ++)
			{
				missions[i].sprite = success;
			}

			money = Convert.ToInt16(money);
		}

		treatsText.text = "x" + money.ToString();
		finish_time.text = Convert.ToInt16((Time.time - startTime) - pauseTime).ToString();

		UpdateMoney();
	}

	//load upgrades from database
	private void LoadUpgardes()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "SELECT * FROM Car WHERE Id=" + carNumber;

		SqliteDataReader reader = command.ExecuteReader();

		List<string> list = new List<string>();
		for (int i = 0; i < 10; i++)
		{
			list.Add(reader[i].ToString());
		}

		dataSources[carNumber - 1] = list.ToArray();

		sqliteConnection.Close();
	}

	//load money
	public void LoadMoney()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);

		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "SELECT * FROM Money WHERE Id=1";

		SqliteDataReader dataReader = command.ExecuteReader();

		money = Convert.ToInt16(dataReader[1]);

		sqliteConnection.Close();

	}

	//update money in database
	private void UpdateMoney()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "UPDATE Money Set treats=" + money.ToString() + " WHERE Id=1";

		command.ExecuteNonQuery();

		sqliteConnection.Close();
	}

	//update the levels
	private void UpdateLevels()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection2);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "UPDATE RaceRiver SET Levels='true' WHERE Id=" + (level + 1);

		command.ExecuteNonQuery();

		sqliteConnection.Close();
	}

	//start numeration
	IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);

        cooldown -= 1;
        StartTimer.text = cooldown.ToString();

        yield return new WaitForSeconds(1);

		cooldown -= 1;
		StartTimer.text = cooldown.ToString();

        yield return new WaitForSeconds(1);

		cooldown -= 1;
		StartTimer.text = cooldown.ToString();

        started = true;
		startTime = Time.time;
        StartTimer.text = "";
	}

	//wait for the finish
	IEnumerator Finish()
	{
		yield return new WaitForSeconds(0.5f);

		started = false;
		isfinished = true;

		end.SetActive(true);
		PlayerPrefs.SetFloat("openedPage", 1);
	}
}
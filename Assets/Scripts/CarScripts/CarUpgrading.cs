using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using Mono.Data.Sqlite;
using System;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class CarUpgrading : MonoBehaviour
{
	[Header("Buttons")]
	public Button[] startbuttons;
	public Button[] endbuttons;

	[Header("Images")]
	public List<Image> acelerationImages;
	public List<Image> speedImages;
	public List<Image> handleImages;

	[Header("Sprites")]
	public Sprite upgradedSprite;

	[Header("Texts")]
	public TextMeshProUGUI[] costs;

	public TextMeshProUGUI moneyText;
	private int money;

	[Header("Effects")]
	public ParticleSystem effects;

	[Header("Panels")]
	public GameObject warningPanel;

	private string connection;
	private string[][] dataSources = new string[2][];
	private Image[][] images = new Image[3][];

	private int carNumber = 1;

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

		//load images to jagged array
		images[0] = acelerationImages.ToArray();
		images[1] = speedImages.ToArray();
		images[2] = handleImages.ToArray();

		LoadFromDatabase();

		//load images for every upgrade
		for (int j = 0; j < 3; j++)
		{
			for (int i = 0; i < Convert.ToInt16(dataSources[carNumber - 1][j + 1]); i++)
			{
				images[j][i].sprite = upgradedSprite;
			}
		}

		LoadMoney();

		effects.Stop();
	}

    void Update()
    {
        
    }

	//opens the endbuttons when clicked
	public void ClickStartButton(int index)
	{
		startbuttons[index].gameObject.SetActive(false);
		endbuttons[index].gameObject.SetActive(true);
	}

	//purchase new upgarde and saves it to database
	public void ClickEndButton(int index)
	{
		int spacenum = 4;

		FirstCar f = new FirstCar();
		int[] restrictions = f.upgrades;

		if (money >= Convert.ToInt16(dataSources[carNumber - 1][spacenum + index]) 
			&& Convert.ToInt16(dataSources[carNumber - 1][1 + index]) < restrictions[index])
		{
			int upgrds = Convert.ToInt16(dataSources[carNumber - 1][1 + index]);

			money -= Convert.ToInt16(dataSources[carNumber - 1][spacenum + index]);
			moneyText.text = money.ToString();

			effects.Play();

			upgrds ++;

			dataSources[carNumber - 1][1 + index] = upgrds.ToString();

			int oldCost = Convert.ToInt16(dataSources[carNumber - 1][spacenum + index]);
			oldCost += 20;

			dataSources[carNumber - 1][spacenum + index] = oldCost.ToString();
			costs[index].text = oldCost.ToString();

			UpdateDatabase();
			UpdateMoney();

			//load images for the upgrades
			for (int j = 0; j < 3; j++)
			{
				for (int i = 0; i < Convert.ToInt16(dataSources[carNumber - 1][j + 1]); i++)
				{
					images[j][i].sprite = upgradedSprite;
				}
			}
		} else
		{
			warningPanel.SetActive(true);

			StartCoroutine(Wait());
		}

		endbuttons[index].gameObject.SetActive(false);
		startbuttons[index].gameObject.SetActive(true);
	}

	//load money from database
	public void LoadMoney()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);

		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "SELECT * FROM Money WHERE Id=1";

		SqliteDataReader dataReader = command.ExecuteReader();

		money = Convert.ToInt16(dataReader[1]);
		moneyText.text = money.ToString();

		sqliteConnection.Close();

	}

	//update the new values of money and save them in database
	public void UpdateMoney()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "UPDATE Money Set treats=" + money.ToString() + " WHERE Id=1";
		
		command.ExecuteNonQuery();

		sqliteConnection.Close();
	}

	//load all car characteristics from database
	private void LoadFromDatabase()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "SELECT * FROM Car WHERE Id=" + carNumber;

		SqliteDataReader reader = command.ExecuteReader();

		List<string> list = new List<string>();
		for (int i = 0; i < 10; i ++)
		{
			list.Add(reader[i].ToString());
		}

		dataSources[carNumber - 1] = list.ToArray();

		sqliteConnection.Close();
	}

	//update the car characteristicks and save them to database
	private void UpdateDatabase()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "UPDATE Car SET AcelerationUpgrade=" + dataSources[carNumber - 1][1]  + ", SpeedUpgrade=" + dataSources[carNumber - 1][2] + ", HandleUpgrade=" + dataSources[carNumber - 1][3] + ", TreatsA=" + dataSources[carNumber - 1][4] + ", TreatsS=" + dataSources[carNumber - 1][5] + ", TreatsH=" + dataSources[carNumber - 1][6] + " WHERE Id=" + carNumber;
	
		command.ExecuteNonQuery();

		sqliteConnection.Close();
	}

	//hide the warning panel
	IEnumerator Wait()
	{
		yield return new WaitForSeconds(2f);

		warningPanel.SetActive(false);
	}
}
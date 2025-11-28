using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

public class ChaptersScene : MonoBehaviour
{
	[Header("Elements")]
	public Image[] lockedMaps;
    public GameObject[] maps;
	public Image[] unlockedMaps;

	[Header("Weather")]
	public Image[] weather_cycle_images;
	public TextMeshProUGUI[] weather_descriptions;
	public Sprite[] weather_colors;
	public Sprite[] day_night;

	[Header("Fuel")]
	public TextMeshProUGUI fuel_text;
	public GameObject noFuel_panel;

	[Header("Levels")]
	public GameObject[] lockedLeves;
	public GameObject[] startLevel;

	[Header("OptionsMenu")]
	public TextMeshProUGUI[] levelnum;

	//private variables
	private string connection;
	private string connection2;
	private string[] levels = new string[20];
	private int levelNum = 0;
	private int num = 0;

	void Start()
    {
		Permission.RequestUserPermission(Permission.ExternalStorageWrite);

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
			connection = "URI=file:" + filepath;
		}
		else
		{
			var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "Levels.db");

			//open db connection
			connection = "URI=file:" + dbPath;
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
			connection2 = "URI=file:" + filepath;
		}
		else
		{
			var dbPath = string.Format(@"Assets/StreamingAssets/{0}", "fuel.db");

			//open db connection
			connection2 = "URI=file:" + dbPath;
		}

		LoadLevels();
		CheckLockedMaps();
		CheckLockedLevels();

		for (int i  = 0; i < maps.Length; i++)
        {
            maps[i].gameObject.SetActive(false);
        }

		Fuel f = new Fuel();

		f.datasource = f.Load_Database(connection2);
		fuel_text.text = f.Load_Fuel(f.datasource).ToString();
	}

    void Update()
    {

	}

	//start button click
	public void StartClick()
	{
		SceneManager.LoadScene("RiverRace");
		PlayerPrefs.SetInt("level", levelNum);

		string[] array = new string[3];

		Fuel f = new Fuel();
		array = f.Load_Database(connection2);

		array[0] = (Convert.ToInt16(array[0]) - 1).ToString();

		if (Convert.ToInt16(array[0]) > 0) f.Save_Database(connection2, array); else noFuel_panel.gameObject.SetActive(true);
	}

	//click on level button
	public void LevelButton(int level)
	{
		startLevel[0].SetActive(true);
		levelNum = level;

		PlayerPrefs.SetInt("level", level);

		levelnum[0].text = "Level " + level.ToString();
		WeatherCycle(level);
	}

	//open maps
    public void OpenMap(int index)
    {
		if (!lockedMaps[index].gameObject.activeSelf)
		{
			maps[index].gameObject.SetActive(true);
		}
	}

	//close maps
	public void CloseMap(int index)
	{
		maps[index].gameObject.SetActive(false);
		startLevel[index].gameObject.SetActive(false);
	}

	//checks the weather and the day cycle
	private void WeatherCycle(int level)
	{
		if (level % 2 != 0)
		{
			//light - day
			weather_descriptions[0].text = "Day /";
			weather_cycle_images[0].sprite = day_night[0];
		}
		else
		{
			//dark - night
			weather_descriptions[0].text = "Night /";
			weather_cycle_images[0].sprite = day_night[1];
		}

		if (level % 3 == 0)
		{
			//rain
			weather_descriptions[1].text = "Rainy";
			weather_cycle_images[1].sprite = weather_colors[1];
		}
		else
		{
			if (level % 2 == 0)
			{
				weather_cycle_images[1].sprite = weather_colors[0];
				weather_descriptions[1].text = "Partly cloudy";
			}
			else
			{
				weather_cycle_images[1].sprite = weather_colors[2];
				weather_descriptions[1].text = "Partly cloudy";
			}
		}
	}

	//check if map is locked
	private void CheckLockedMaps()
	{
		if (levels[10] == "true")
		{
			for (int i = 0; i < 2; i++)
			{
				lockedMaps[i].gameObject.SetActive(false);
				unlockedMaps[i].color = new Color(120, 120, 120, 255);
			}
		}

		lockedMaps[0].gameObject.SetActive(false);
	}

	//check if level is locked
	private void CheckLockedLevels()
	{
		for (int i = 0; i < lockedLeves.Length; i++)
		{
			if (levels[i] == "false")
			{
				lockedLeves[i].SetActive(true);
			}
		}
	}

	//load levels
	private void LoadLevels()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand cmd = sqliteConnection.CreateCommand();
		cmd.CommandText = "SELECT count(Levels) FROM RaceRiver";

		SqliteDataReader dataReader = cmd.ExecuteReader();

		int num = Convert.ToInt16(dataReader[0]);;

		dataReader.Close();

		cmd.CommandText = "SELECT Levels FROM RaceRiver";
		
		dataReader = cmd.ExecuteReader();

		for (int i = 0; i < num; i ++)
		{
			dataReader.Read();
			levels[i] = dataReader[0].ToString();
		}

		sqliteConnection.Close();
	}
}
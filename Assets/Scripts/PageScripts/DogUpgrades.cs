using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DogUpgrades : MonoBehaviour
{
	[Header("Upgrades")]
	public GameObject Upgrades;
	public Sprite upgraded;
	public Sprite unupgraded;

	public TextMeshProUGUI cost_text;
	public TextMeshProUGUI money_text;

	[Header("Warning panel")]
	public GameObject warning_panel;

	[Header("Buttons")]
	public GameObject show_button;
	public GameObject real_button;

	private Image[] upgrades;
	private string connection;
	private int num = 0;
	private int cost = 450;
	private int money;

	private string[][] dataSources = new string[2][];

	void Start()
    {
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

		List<Image> ug = new List<Image>();
		foreach (Image i in Upgrades.GetComponentsInChildren<Image>())
		{
			ug.Add(i);
		}

		upgrades = ug.ToArray();

		for (int i = 0; i <  upgrades.Length; i++)
		{ 
			upgrades[i].sprite = unupgraded;
		}

		Get_Upgrades();
		Load_Upgrades();
		Load_Money();

		money_text.text = money.ToString();
	}

    void Update()
    {
        
    }

	public void Show_Button()
	{
		real_button.gameObject.SetActive(true);
		show_button.gameObject.SetActive(false);
	}

	public void Upgrade()
	{
		if (money >= Convert.ToInt16(cost_text) && num != upgrades.Length)
		{
			money -= Convert.ToInt16(cost_text.text);
			num ++;

			upgrades[num - 1].sprite = upgraded;
			cost += 30;

			cost_text.text = cost.ToString();
			money_text.text = money.ToString();

			Update_Upgrades();
			Upgrade_Money();
		} else
		{
			warning_panel.SetActive(true);
			StartCoroutine(Wait());
		}

		Hide_Button();
	}

	private void Hide_Button()
	{
		real_button.gameObject.SetActive(false);
		show_button.gameObject.SetActive(true);
	}

	private void Upgrade_Money()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "UPDATE Money Set treats=" + money.ToString() + " WHERE Id=1";

		command.ExecuteNonQuery();

		sqliteConnection.Close();
	}

	private void Update_Upgrades()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "UPDATE Car SET SPupgrades=" + num + " , SPvalue=" + cost + " WHERE Id=1";

		command.ExecuteNonQuery();
		sqliteConnection.Close();
	}

	private void Load_Money()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);

		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "SELECT * FROM Money WHERE Id=1";

		SqliteDataReader dataReader = command.ExecuteReader();

		money = Convert.ToInt16(dataReader[1]);

		sqliteConnection.Close();
	}

	private void Load_Upgrades()
	{
		for (int i = 0; i < Convert.ToInt16(dataSources[0][8]); i ++)
		{
			upgrades[i].sprite = upgraded;
			num++;
		}
	}

	private void Get_Upgrades()
	{
		SqliteConnection sqliteConnection = new SqliteConnection(connection);
		sqliteConnection.Open();

		SqliteCommand command = sqliteConnection.CreateCommand();
		command.CommandText = "SELECT * FROM Car WHERE Id=1";

		SqliteDataReader reader = command.ExecuteReader();

		List<string> st = new List<string>();
		for (int i = 0; i < 10; i ++)
		{
			st.Add(reader[i].ToString());
		}

		dataSources[0] = st.ToArray();

		sqliteConnection.Close();
	}

	IEnumerator Wait()
	{
		yield return new WaitForSeconds(2);
		warning_panel.SetActive(false);
	}
}
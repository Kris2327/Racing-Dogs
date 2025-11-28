using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class Page_Exit : MonoBehaviour
{
	[Header("Fuel")]
	public TextMeshProUGUI fuel_text;
	public double time_left;

	private string connection;

	void Start()
	{
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

	private void OnApplicationQuit()
	{
		if (fuel_text.gameObject != null)
		{
			string[] array = new string[3];

			array[0] = fuel_text.text;
			array[1] = DateTime.Now.ToString("H:mm");
			array[2] = time_left.ToString();
			 
			Fuel f = new Fuel();
			f.Save_Database(connection, array);
			PlayerPrefs.SetString("date", f.CheckDate());
		} else
		{

		}
	}
}

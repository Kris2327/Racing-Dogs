using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mono.Data.Sqlite;

public class Fuel
{
	public string time_now;
	public string date_now;

	public double fuel;
	public double time_left;

	public string[] datasource = new string[4];
	public bool loaded = false;

	public void Save_Database(string path, string[] name)
	{
		SqliteConnection connection = new SqliteConnection(path);
		connection.Open();

		SqliteCommand command = connection.CreateCommand();
		command.CommandText = "UPDATE Fuel SET Fuel='" + name[0] + "', Time_Close='" + name[1] + "', Time_Left='" + Math.Floor(Convert.ToDouble(name[2])).ToString() + "' WHERE Id=1;";

		command.ExecuteNonQuery();
		connection.Close();
	}

	public double Get_Time(string path)
	{
		if(!loaded) datasource = Load_Database(path); loaded = true;
		fuel = Load_Fuel(datasource);

		string time = datasource[2];
		string date = PlayerPrefs.GetString("date");

		time_now = CheckTime();
		date_now = CheckDate();

		if (date_now.Equals(date) && fuel < 10 && Convert.ToInt16(datasource[2]) != 0)
		{
			int h = time_now.IndexOf(":");
			int hours = Convert.ToInt16(time_now.Substring(0, h));
			int min = Convert.ToInt16(time_now.Substring(h).Replace(":", ""));
			
			int h_first = time.IndexOf(":");
			int hours_first = Convert.ToInt16(time.Substring(0, h_first));
			int min_first = Convert.ToInt16(time.Substring(h_first).Replace(":", ""));

			double time_estimated = (hours * 60 + min) - (hours_first * 60 + min_first);

			fuel += Math.Floor(time_estimated / 15);

			time_left = 15 - (time_estimated - fuel * 15);
		}
        else
        {
			fuel = 10;
        }

		if (fuel > 10)
		{
			fuel = 10;
		}

		PlayerPrefs.SetInt("fuel", (int)fuel);

		if (Convert.ToInt16(datasource[2]) != 0)
		{
			return Convert.ToInt16(datasource[2]);
		} else
		{
			return time_left;
		}
    }

	public string[] Load_Database(string path)
	{
		SqliteConnection connection = new SqliteConnection(path);
		connection.Open();

		SqliteCommand command = connection.CreateCommand();
		command.CommandText = "SELECT * FROM Fuel";

		SqliteDataReader reader = command.ExecuteReader();

		List<string> list = new List<string>();
		for (int i = 0; i < 4; i++)
		{
			list.Add(reader[i].ToString());
		}

		list.Remove(list[0]);

		datasource = list.ToArray();

		connection.Close();

		return datasource;
	}

	public int Load_Fuel(string[] source)
	{
		return Convert.ToInt16(source[0]);
	}

	public string CheckTime()
	{
		return DateTime.Now.ToString("H:mm");
	}

	public string CheckDate()
	{
		return DateTime.Now.ToString("MM / dd / yyyy");
	}
}

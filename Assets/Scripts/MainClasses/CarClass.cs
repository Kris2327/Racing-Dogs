using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarClass
{
	public string connection;
	public float numA;
	public float numS;
	public float numH;

	public void MoveForward(Rigidbody rb, Transform car, bool isfinished, bool grounded, float speed, float aceleration)
	{
		Vector3 vector3 = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		if (vector3.magnitude <= speed / 15 && grounded && isfinished == false)
		{
			//move forward
			rb.AddForce(-car.right * aceleration * 350 * Time.deltaTime, ForceMode.Impulse);
		}
	}

	public void RotateRight(Transform car, float handle)
	{
		car.Rotate(Vector3.up, -handle * Time.deltaTime);
	}

	public void RotateLeft(Transform car, float handle)
	{
		car.Rotate(Vector3.up, handle * Time.deltaTime);
	}

	public void TimeRaces(bool isfinished, TextMeshProUGUI time, float Ftime, float startTime, GameObject endScene, TextMeshProUGUI EndTime,
	GameObject right_arrow, GameObject left_arrow)
	{
		if (isfinished == false)
		{
			time.text = "Time: " + String.Format("{0:00:00}", (Time.time - startTime) * 100);

			Ftime = Time.time;
		}
		else
		{
			endScene.SetActive(true);
			right_arrow.SetActive(false);
			left_arrow.SetActive(false);

			EndTime.text = time.text;
		}
	}

	public void CollsisionTrees(Rigidbody car)
	{
		car.AddForce(-car.transform.position / 10);
	}
}

//first car parameters
public class FirstCar
{
	public float acerelation = 100f;
	public float speed = 260f;
	public float driftSpeed = 250f;
	public float handle = 30f;
	public int[] upgrades = { 3, 3, 4 };
	public int SuperPUpgrades = 3;
}

//second car parameters
public class SecondCar
{
	public float acerelation = 120f;
	public float speed = 270f;
	public float driftSpeed = 260f;
	public float handle = 35f;
	public int[] upgrades = { 4, 3, 4 };
	public int SuperPUpgrades = 3;
}
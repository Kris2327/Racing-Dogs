using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
	public GameObject[] cars;
	public GameObject[] orientations;

	void Start()
	{
		/*for (int i = 0; i < cars.Length; i++)
		{
			cars[i].SetActive(false);
		}

		cars[Convert.ToInt16(PlayerPrefs.GetFloat("selectedCar"))].SetActive(true);*/
	}

	void Update()
	{

	}

	private void FixedUpdate()
	{
		//add cars[Convert.ToInt16(PlayerPrefs.GetFloat("selectedCar"))]. when multiple cars
		transform.LookAt(cars[0].transform);
		float car_Move = Mathf.Abs(Vector3.Distance(transform.position, orientations[0].transform.position) * 3f);
		transform.position = Vector3.MoveTowards(transform.position, orientations[0].transform.position, car_Move * Time.deltaTime);
	}
}

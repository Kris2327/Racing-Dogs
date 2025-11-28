using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageScript : MonoBehaviour
{
    public GameObject[] cars;
	public GameObject locked;
	public GameObject carAnimation;

    int carsCount = 0;

    void Start()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
        }

        cars[0].SetActive(true);
    }

    public void RightArrow()
    {
        if (carsCount < 1)
        {
			carsCount++;

			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].SetActive(false);
			}

			cars[carsCount].SetActive(true);

			if (carsCount == 1)
			{
				locked.SetActive(true);
			} else
			{
				locked.SetActive(false);
				carAnimation.SetActive(false);
			}
		}
    }

	public void LeftArrow()
	{
		if (carsCount > 0)
		{
			carsCount--;

			for (int i = 0; i < cars.Length; i++)
			{
				cars[i].SetActive(false);
			}

			cars[carsCount].SetActive(true);

			if (carsCount == 1)
			{
				locked.SetActive(true);
			}
			else
			{
				locked.SetActive(false);
				carAnimation.SetActive(false);
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyCarMovementScript : MonoBehaviour
{
    //public variables
    [Header("Car")]
	public Rigidbody carRb;
	public float carHeight;
	public LayerMask whatisground;
	public ParticleSystem[] particles;
	public TrailRenderer[] trails;
    public float angle;
	public Image health;

	[Header("Pause")]
    public GameObject PauseMenu;
    public GameObject BadEnd;

    [Header("Checkpoints")]
    public GameObject checkpointParent;
    public GameObject cube;

    [Header("Player")]
    public GameObject player;
    public GameObject healthBar;

    private float speed;
	private float aceleration;
    private float driftSpeed;
    private float follow_speed = 25f;

    private bool grounded;
    private bool started = false;
	bool isfinished = false;
    int num = 0;
    int level = 0;
    int followNum = 0;

    //arrays
    public GameObject[] checkpoints;

	void Start()
    {
        FirstCar f = new FirstCar();

		level = PlayerPrefs.GetInt("level");

        if (level == 10)
        {
            if (transform.gameObject.name != "Boss")
            {
                transform.gameObject.SetActive(false);
            }
        }

		speed = f.speed + 10f + (level - 1) * 5;
        driftSpeed = f.driftSpeed + 10f + (level - 1) * 5;
        aceleration = f.acerelation + (level - 1) * 5;

        for (int i = 0; i < trails.Length; i++)
        {
            trails[i].emitting = false;
        }

        List<GameObject> list = new List<GameObject>();
        foreach (Transform g in checkpointParent.GetComponentsInChildren<Transform>())
        {
            list.Add(g.gameObject);
        }

        list.Remove(list[0]);

        checkpoints = list.ToArray();

        if (level == 4 || level == 8 || level == 9)
        {
            transform.gameObject.SetActive(false);
        }

        if (level == 3 || level == 7) 
        {
            healthBar.SetActive(true);
			StartCoroutine(Wait3());

            if (level == 3) followNum = 0;
            if (level == 7) followNum = 1;
		} else
        {
            StartCoroutine(Wait());
        }
    }

    void Update()
    {
		grounded = Physics.Raycast(transform.position, Vector3.down, carHeight * 0.5f + 0.2f, whatisground);

        if (level == 3 || level == 7)
        {
			if (started && !PauseMenu.activeSelf) FollowPlayer();
		} else
        {
			if (started && !PauseMenu.activeSelf)
			{
				CarClass carsMovement = new CarClass();

				if (SceneManager.GetActiveScene().name == "snowmap")
				{
					if (!trails[0].emitting) carsMovement.MoveForward(carRb, transform, isfinished, grounded, speed + 50f, aceleration + 20f);
					else carsMovement.MoveForward(carRb, transform, isfinished, grounded, driftSpeed + 50f, aceleration + 20f);
				}
				else
				{
					if (!trails[0].emitting) carsMovement.MoveForward(carRb, transform, isfinished, grounded, speed, aceleration);
					else carsMovement.MoveForward(carRb, transform, isfinished, grounded, driftSpeed, aceleration);
				}

				if (transform.eulerAngles.y != checkpoints[num].transform.eulerAngles.y) CheckRotation();
			}
		}        

		if (grounded)
		{
			if (transform.rotation != Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z))
			{
				transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);
			}
		}
	}

    //if the car collides with object
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "CheckPoint" && transform.gameObject.name == "Enemy1")
        {
            other.gameObject.SetActive(false);
            num++;

            if (num % 2 == 0)
            {
				carRb.AddForce(Vector3.back * 150f * checkpoints[num - 1].transform.position.x * Time.deltaTime, ForceMode.Impulse);
			} else
            {
				carRb.AddForce(Vector3.forward * 150f * checkpoints[num - 1].transform.position.x * Time.deltaTime, ForceMode.Impulse);
			}
        }

		if (other.tag == "CheckPoint2" && transform.gameObject.name == "Enemy2")
		{
			other.gameObject.SetActive(false);
			num++;


			if (num % 2 == 0)
			{
				carRb.AddForce(Vector3.back * 150f * checkpoints[num - 1].transform.position.x * Time.deltaTime, ForceMode.Impulse);
			}
			else
			{
				carRb.AddForce(Vector3.forward * 150f * checkpoints[num - 1].transform.position.x * Time.deltaTime, ForceMode.Impulse);
			}
		}

		if (other.tag == "CheckPoint3" && transform.gameObject.name == "Boss")
		{
			other.gameObject.SetActive(false);
			num++;


			if (num % 2 == 0)
			{
				carRb.AddForce(Vector3.back * 150f * checkpoints[num - 1].transform.position.x * Time.deltaTime, ForceMode.Impulse);
			}
			else
			{
				carRb.AddForce(Vector3.forward * 150f * checkpoints[num - 1].transform.position.x * Time.deltaTime, ForceMode.Impulse);
			}
		}

		if (other.tag == "finish")
        {
            cube.SetActive(false);
            transform.name = "finished1";
            player.name = "unfinished";

            StartCoroutine(Wait2());
        }
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == player)
        {
			if (level == 3 || level == 7)
            {
				health.fillAmount -= 2 * Time.deltaTime;

				if (health.fillAmount <= 0)
				{
					Destroy(transform.gameObject);
					BadEnd.SetActive(true);
					started = false;
				}

				transform.Translate(Vector3.forward * 100 * Time.deltaTime);
			}
		}

        if (collision.gameObject.name == "Enemy2")
        {
            if (level == 3 || level == 7) transform.Translate(Vector3.right * 100 * Time.deltaTime);
        }
	}

	//rotate the car if in need
	private void CheckRotation()
    {
        if (transform.eulerAngles.y >= checkpoints[num].transform.eulerAngles.y - 1 && transform.eulerAngles.y <= checkpoints[num].transform.eulerAngles.y + 1)
        {
            for (int i = 0; i < trails.Length; i ++)
            {
                trails[i].emitting = false;
            }

            if (!particles[0].IsAlive())
            {
				for (int i = 0; i < particles.Length; i++)
				{
                    particles[i].Play();
				}

                StartCoroutine(WaitFire());
			}

        } else
        {
			if (transform.eulerAngles.y > checkpoints[num].transform.eulerAngles.y || num == 20)
			{
				transform.Rotate(Vector3.down, (40f + level * 1.5f) * Time.deltaTime);

				for (int i = 0; i < trails.Length; i++)
				{
					trails[i].emitting = true;
				}
			}
			
            if (transform.eulerAngles.y < checkpoints[num].transform.eulerAngles.y)
			{
				transform.Rotate(Vector3.up, (40f + level * 1.5f) * Time.deltaTime);

				for (int i = 0; i < trails.Length; i++)
				{
					trails[i].emitting = true;
				}
			}
		}
	}
    
    //follow the player
    private void FollowPlayer()
    {
        Vector3 vector = Vector3.MoveTowards(transform.position, player.transform.position, (follow_speed + followNum * 2) * Time.deltaTime);

        carRb.MovePosition(vector);

        if (transform.eulerAngles.y >= player.transform.eulerAngles.y - 1 && transform.eulerAngles.y <= player.transform.eulerAngles.y + 1)
        {
            for (int i = 0; i < trails.Length; i ++)
            {
                trails[i].emitting = false;
            }

            follow_speed = 25f;
        }else
        {
            if (transform.eulerAngles.y > player.transform.eulerAngles.y)
            {
                transform.Rotate(Vector3.down, 0.5f);

                for (int i = 0; i < trails.Length; i ++)
                {
                    trails[i].emitting = true;
                }
            } else if (transform.eulerAngles.y < player.transform.eulerAngles.y)
            {
                transform.Rotate(Vector3.up, 0.5f);

                for (int i = 0; i < trails.Length; i ++)
                {
                    trails[i].emitting = true;
                }
            }

            follow_speed = 20f;
        }
    }

    //wait in the begining
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(3);
        started = true;
    }

    IEnumerator Wait2()
    {
        yield return new WaitForSeconds(0.5f);
        started = false;
    }

    IEnumerator Wait3()
    {
        yield return new WaitForSeconds(5f);
        started = true;
    }

    IEnumerator WaitFire()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < particles.Length; i ++)
        {
            particles[i].Stop();
        }
    }
}
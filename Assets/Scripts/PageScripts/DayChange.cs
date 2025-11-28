using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayChange : MonoBehaviour
{
    [Header("Sky")]
    public Material LightSky;
    public Material DarkSky;

    [Header("Light")]
    public Light main_light;

    [Header("CarLights")]
    public List<GameObject> lights;

    [Header("Rain")]
    public ParticleSystem rain;

    private int level;

    // Start is called before the first frame update
    void Start()
    {
        level = PlayerPrefs.GetInt("level");

        if (level % 2 != 0)
        {
            //light
            RenderSettings.skybox = LightSky;
            main_light.color = Color.white;

            for (int i = 0; i < lights.Count; i ++)
            {
                lights[i].SetActive(false);
            }
		} else
        {
            //dark
			RenderSettings.skybox = DarkSky;
            main_light.color = Color.black;

			for (int i = 0; i < lights.Count; i++)
			{
				lights[i].SetActive(false);
			}
		}

        if (level % 3 == 0)
        {
            rain.Play();
        } else
        {
            rain.Stop();
        }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

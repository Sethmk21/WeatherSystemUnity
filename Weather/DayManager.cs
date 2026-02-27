using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Globalization;
public class DayManager : MonoBehaviour
{
    public Material skybox;
    public float timeOfDay = 20;
    public float timer = 0;
    public bool isDay = true;
    private Light dLight;
    private float dLightInt = 0;
    private float nightOffset = 0;
    public WeatherState currentWeatherState;
    private bool once = false;
    public float intmult = 1;
    public int numOfRain = 0;
    public Color nightColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skybox.SetFloat("_DayNight", 0);
        dLight = GetComponent<Light>();
        currentWeatherState = GetComponent<SunnyState>();
        currentWeatherState.StartState();
        dLight.color = currentWeatherState.weatherColor;
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        timeOfDay += Time.fixedDeltaTime;
        
        transform.rotation = Quaternion.Euler(timeOfDay * 0.5f + 10, -90, 0);
        if (timeOfDay >= 720)
        {
            timeOfDay = 0;
        }
        if (transform.rotation.eulerAngles.x % 360 >= 180 && isDay == true)
        {
            isDay = false;
            dLight.intensity = 1f;
            nightOffset = 0.4f;
            //StartCoroutine(FadeOutLight(0.5f));
        }
        else if (transform.rotation.eulerAngles.x % 360 <= 180 && isDay == false)
        {
            isDay = true;
            dLight.intensity = 1f;
            nightOffset = 0f;
            //StartCoroutine(FadeInLight(1.5f));
        }
        if ((timeOfDay % 30) > 0  && (timeOfDay % 30) < 1 && once == false)
        {
            //print("new weather");
            once = true;
            if (numOfRain < 4)
            {
                Invoke("changeOnce", 2);
                WeatherState former = currentWeatherState;
                currentWeatherState = currentWeatherState.GetNextState();
                if (former != currentWeatherState)
                {
                    currentWeatherState.StartState();
                    StartCoroutine(FadeColorRoutine(currentWeatherState.weatherColor, 10));
                    fireManager.Instance.weatherBasedR = currentWeatherState.wbr;
                }
            }
            else
            {
                currentWeatherState = GetComponent<SunnyState>();
            }
        }
        if (currentWeatherState != null) 
        {
            currentWeatherState.DoWeather();
        }
        if(isDay == true && timeOfDay > 270)
        {
            skybox.SetFloat("_DayNight", (timeOfDay-270f) / 90f);
        }
        if (isDay == false && timeOfDay > 630)
        {
            skybox.SetFloat("_DayNight", (720f - timeOfDay) / 90f);
        }
    }
    private void changeOnce()
    {
        once = false;
    }
    void Update()
    {
        
        dLightInt = Mathf.Sin(transform.rotation.eulerAngles.x * math.PI *0.0055556f) * 0.5f ; // Amplitude of 0.5 units
        dLight.intensity =  intmult * (dLightInt + 1) - nightOffset;
    }
    private IEnumerator FadeColorRoutine(Color endColor, int duration)
    {
        if (isDay)
        {
            float timeElapsed = 0;

            while (timeElapsed < duration)
            {
                // Calculate the new color using Color.Lerp
                Color newColor = Color.Lerp(dLight.color, endColor, timeElapsed / duration);

                dLight.color = newColor;
                //skybox.color = newColor;
                timeElapsed += Time.deltaTime;
                yield return null; // Wait until the next frame
            }
            //dLight.color = endColor;
        }
        else 
        {
            float timeElapsed = 0;

            while (timeElapsed < duration)
            {
                // Calculate the new color using Color.Lerp
                Color newColor = Color.Lerp(dLight.color, nightColor, timeElapsed / duration);

                dLight.color = newColor;
               // skybox.color = newColor;
             
                timeElapsed += Time.deltaTime;
                yield return null; // Wait until the next frame
            }
            //dLight.color = endColor;
        }
        // Ensure the final color is set precisely
    }

}

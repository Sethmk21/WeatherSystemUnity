using UnityEditor;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeatherState : MonoBehaviour
{
    public GameObject ps;
    public float lightIntensityMult;
    public WeatherState[] availableWeatherStates;
    public Color weatherColor;
    public DayManager dayManager;
    public float wbr = 1;
    public WeatherState()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void StartState()
    {
        print("Start State");
        StartCoroutine(changelight());
        if (ps != null)
        {
            ps.SetActive(true);
        }
    }
    public virtual void DoWeather()
    {

    }

    public virtual WeatherState GetNextState()
    {
        return availableWeatherStates[0];
    }
    IEnumerator changelight()
    {
        while (dayManager.intmult < lightIntensityMult)
        {
            //print("smaller");
            dayManager.intmult += Time.deltaTime / 50;
            yield return null;
        }
        while (dayManager.intmult > lightIntensityMult)
        {
            dayManager.intmult -= Time.deltaTime / 50;
            yield return null;
        }
        dayManager.intmult = lightIntensityMult;
        yield break;
    }

}
